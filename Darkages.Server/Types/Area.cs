using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Darkages
{
    public class Area : ObjectManager
    {
        [JsonIgnore] private static readonly byte[] sotp = File.ReadAllBytes("sotp.dat");

        [Browsable(false)]
        public byte[] Data { get; set; }

        [Browsable(false)]
        public ushort Hash;

        [JsonIgnore]
        [Browsable(false)]
        public TileContent[,] Tile { get; set; }

        public int Music { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public bool Ready { get; set; }

        [JsonRequired]
        public ushort Rows { get; set; }

        [JsonRequired]
        public ushort Cols { get; set; }

        [JsonRequired]
        public int Number { get; set; }

        public int ID { get; set; }
        public string Name { get; set; }
        public MapFlags Flags { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public bool Sent { get; set; }

        [JsonIgnore] [Browsable(false)]
        private GameServerTimer WarpTimer =
            new GameServerTimer(TimeSpan.FromSeconds(ServerContext.Config.WarpUpdateTimer));

        public static bool ParseSotp(short lWall, short rWall)
        {
            if (lWall == 0 &&
                rWall == 0)
                return false;
            if (lWall == 0)
                return sotp[rWall - 1] == 0x0F;
            if (rWall == 0)
                return sotp[lWall - 1] == 0x0F;
            return
                sotp[lWall - 1] == 0x0F &&
                sotp[rWall - 1] == 0x0F;
        }

        public bool IsWall(Sprite obj, int x, int y)
        {
            if (x < 0 || y < 0)
                return true;

            x = x.Clamp(x, Cols - 1);
            y = y.Clamp(y, Rows - 1);

            if (obj is Aisling)
            {
                if (((Aisling) obj).Flags.HasFlag(AislingFlags.GM))
                {
                    return false;
                }

                SetWarps();

                if (Tile[x, y] == TileContent.Warp)
                    return false;

                var isobj = Tile[x, y];

                if (isobj == TileContent.Monster || isobj == TileContent.Aisling && GetObject(i => i != null && i.X == x && i.Y == y,
                        Get.Aislings | Get.Monsters | Get.Mundanes) == null)
                {
                    Tile[x, y] = isobj == TileContent.Wall
                        ? TileContent.Wall
                        : TileContent.None;

                    return false;
                }

                if (Tile[x, y] != TileContent.Wall)
                    if ((obj as Aisling).Dead)
                        return false;
            }

            if (obj is Monster)
            {
                if (((Monster) obj).Template.IgnoreCollision)
                    return false;
            }


            foreach (var nobj in GetObjects(i => i != null && i.X == x && i.Y == y,
                Get.Monsters | Get.Mundanes | Get.Aislings))
                Tile[nobj.X, nobj.Y] = nobj.Content;

            if (Tile[x, y] == TileContent.Warp)
                return true;
            if (Tile[x, y] == TileContent.Wall)
                return true;
            if (Tile[x, y] == TileContent.Monster)
                return true;
            if (Tile[x, y] == TileContent.Mundane)
                return true;
            if (Tile[x, y] == TileContent.Aisling)
                return true;
            if (Tile[x, y] == TileContent.None)
                return false;

            return false;
        }

        public byte[] GetRowData(int row)
        {
            var buffer = new byte[Cols * 6];

            unsafe
            {
                fixed (byte* lpData = buffer, lpTile = &Data[row * Cols * 6])
                {
                    var lpD = lpData;
                    var lpT = lpTile;

                    for (var i = 0; i < Cols; i++, lpD += 6, lpT += 6)
                    {
                        lpD[0] = lpT[1];
                        lpD[1] = lpT[0];

                        lpD[2] = lpT[3];
                        lpD[3] = lpT[2];

                        lpD[4] = lpT[5];
                        lpD[5] = lpT[4];
                    }
                }
            }

            return buffer;
        }

        public void Update(TimeSpan elapsedTime)
        {
            if (!Has<Aisling>())
                return;

            if (Has<Monster>())
                UpdateMonsters(elapsedTime);

            if (Has<Mundane>())
                UpdateMundanes(elapsedTime);

            if (Has<Item>() || Has<Money>())
                UpdateItems(elapsedTime);

            WarpTimer.Update(elapsedTime);
            if (WarpTimer.Elapsed)
            {
                UpdateWarps();
                WarpTimer.Reset();
            }
        }

        private void UpdateWarps()
        {
            var warpsOnMap = ServerContext.GlobalWarpTemplateCache.Where(i => i.ActivationMapId == ID);

            foreach (var warp in warpsOnMap)
            {
                if (!Has<Aisling>())
                    continue;

                if (!warp.Activations.Any())
                    continue;

                var nearby = GetObjects<Aisling>(i => 
                    i.LoggedIn && i.CurrentMapId == warp.ActivationMapId);

                if (nearby.Length == 0)
                    continue;


                foreach (var warpObj in warp.Activations)
                {
                    foreach (var obj in nearby)
                    {
                        if (obj.Position.WithinSquare(warpObj.Location, 10))
                        {
                            obj.Show(Scope.Self, new ServerFormat29(
                                ServerContext.Config.WarpAnimationNumber, 
                                warpObj.Location.X, warpObj.Location.Y));
                        }
                    }
                }
            }
        }

        private void UpdateMonsters(TimeSpan elapsedTime)
        {
            foreach (var obj in GetObjects<Monster>(i => i.CurrentMapId == ID))
            {
                var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(obj) && i.CurrentMapId == ID).Length;

                if (nearby == 0)
                    continue;

                if (obj != null && obj.Map != null && obj.Script != null)
                {
                    obj.Script.Update(elapsedTime);
                    obj.UpdateBuffs(elapsedTime);
                    obj.UpdateDebuffs(elapsedTime);
                    SaveObject(obj);
                }
            }
        }

        private void UpdateItems(TimeSpan elapsedTime)
        {
            foreach (var obj in GetObjects(i => i.CurrentMapId == ID, Get.Items | Get.Money))
            {
                var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(obj) && i.CurrentMapId == ID).Length;

                if (nearby == 0)
                    continue;

                if (obj != null)
                {
                    if (obj is Money)
                        SaveObject(obj as Money);

                    if (obj is Item)
                        SaveObject(obj as Item);
                }
            }
        }

        private void UpdateMundanes(TimeSpan elapsedTime)
        {
            var objects = GetObjects<Mundane>(i => i != null && i.CurrentMapId == ID);
            if (objects == null)
                return;

            foreach (var obj in objects)
            {
                if (obj == null)
                    continue;

                var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(obj) && i.CurrentMapId == ID).Length;

                if (nearby == 0)
                    continue;

                SaveObject(obj);
                obj.UpdateBuffs(elapsedTime);
                obj.UpdateDebuffs(elapsedTime);
                obj.Update(elapsedTime);
            }
        }

        public bool Has<T>()
            where T : Sprite, new()
        {
            var objs = GetObjects<T>(i => i != null && i.CurrentMapId == ID);

            return objs.Length > 0;
        }

        public Position[] GetNearByTiles(Sprite obj, short x, short y, double radius)
        {
            var innerBound = radius * (Math.Sqrt(2.0) / 2.0);
            var radiusSq = radius * radius;
            var result = new List<Position>();

            for (var j = 0; y < Rows; y++)
                for (var i = 0; x < Cols; x++)
                {

                    var xDist = Math.Abs(x - i);
                    var yDist = Math.Abs(y - j);

                    if (xDist > radius || yDist > radius)
                        continue;
                    if (xDist > innerBound || yDist > innerBound)
                        continue;
                    if (IsWall(obj, i, j))
                        continue;
                    if (i == x && j == y)
                        continue;


                    if (new Position(x, y).DistanceFrom(new Position(i, j)) < radiusSq
                        && Tile[i, j] == TileContent.None)
                        result.Add(new Position(i, j));
                }

            return result.ToArray();
        }

        public void OnLoaded()
        {

            Tile = new TileContent[Cols, Rows];

            using (var stream = new MemoryStream(Data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    for (int y = 0; y < Rows; y++)
                    {
                        for (int x = 0; x < Cols; x++)
                        {

                            reader.BaseStream.Seek(2, SeekOrigin.Current);

                            if (ParseSotp(reader.ReadInt16(), reader.ReadInt16()))
                                Tile[x, y] = TileContent.Wall;
                            else
                                Tile[x, y] = TileContent.None;

                        }
                    }
                    SetWarps();
                }
            }

            Ready = true;
        }

        private void SetWarps()
        {

            var warps = ServerContext.GlobalWarpTemplateCache
                .Where(i => i.ActivationMapId == ID).ToArray();

            if (warps.Length == 0)
                return;

            foreach (var warp in warps)
            {
                foreach (var o in warp.Activations)
                {
                    if (warp.WarpType == WarpType.Map)
                    {
                        Tile[o.Location.X, o.Location.Y] = TileContent.Warp;
                    }
                }
            }

        }

        public Position FindNearestEmpty(Position aislingPosition)
        {
            var positions = new List<Position>();

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {

                    if (Tile[x, y] == TileContent.None
                        || Tile[x, y] == TileContent.Money
                        || Tile[x, y] == TileContent.Item)
                    {
                        positions.Add(new Position(x, y));
                    }
                }
            }

            return positions.OrderBy(i => i.DistanceFrom(aislingPosition))
                .FirstOrDefault();
        }
    }
}