using Darkages.Common;
using Darkages.Network;
using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Darkages.Types.ElementManager;

namespace Darkages.Types
{
    public abstract class Sprite : ObjectManager
    {
        [JsonIgnore]
        public int[][] facingTable = {
                       new int[] { -1, +3, -1 },
                       new int[] { +0, -1, +2 },
                       new int[] { -1, +1, -1 },
        };

        [JsonIgnore]
        public int[][] Directions = {
                        new int[] { +0, -1 },
                        new int[] { +1, +0 },
                        new int[] { +0, +1 },
                        new int[] { -1, +0 },
        };

        [JsonIgnoreAttribute]
        public GameClient Client { get; set; }

        [JsonIgnoreAttribute]
        public Area Map { get; set; }

        [JsonIgnoreAttribute]
        public TileContent Content { get; private set; }

        public List<Debuff> Debuffs { get; set; }

        public List<Buff> Buffs { get; set; }

        public int Serial { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public byte Direction { get; set; }

        public int CurrentMapId { get; set; }

        public Element OffenseElement { get; set; }

        public Element DefenseElement { get; set; }

        #region Attributes
        public int CurrentHp { get; set; }

        public int CurrentMp { get; set; }

        public int _MaximumHp { get; set; }

        public int _MaximumMp { get; set; }

        [JsonIgnore]
        public int MaximumHp => _MaximumHp + BonusHp;

        [JsonIgnore]
        public int MaximumMp => _MaximumMp + BonusMp;

        public byte  _Str { get; set; }

        public byte  _Int { get; set; }

        public byte  _Wis { get; set; }

        public byte  _Con { get; set; }

        public byte  _Dex { get; set; }

        public sbyte _Ac { get; set; }

        public byte  _Mr { get; set; }

        public byte  _Dmg { get; set; }

        public byte  _Hit { get; set; }

        [JsonIgnore]
        public byte Str => (byte)(_Str + BonusStr);
        [JsonIgnore]
        public byte Int => (byte)(_Int + BonusInt);
        [JsonIgnore]
        public byte Wis => (byte)(_Wis + BonusWis);
        [JsonIgnore]
        public byte Con => (byte)(_Con + BonusCon);
        [JsonIgnore]
        public byte Dex => (byte)(_Dex + BonusDex);
        [JsonIgnore]
        public byte Ac  => (byte)(_Ac  + BonusAc);
        [JsonIgnore]
        public byte Mr  => (byte)(_Mr  + BonusMr);
        [JsonIgnore]
        public byte Dmg => (byte)(_Dmg + BonusDmg);
        [JsonIgnore]
        public byte Hit => (byte)(_Hit + BonusHit);

        [JsonIgnore]
        public byte BonusStr { get; set; }
        [JsonIgnore]
        public byte BonusInt { get; set; }
        [JsonIgnore]
        public byte BonusWis { get; set; }
        [JsonIgnore]
        public byte BonusCon { get; set; }
        [JsonIgnore]
        public byte BonusDex { get; set; }
        [JsonIgnore]
        public byte BonusMr { get; set; }
        [JsonIgnore]
        public sbyte BonusAc { get; set; }
        [JsonIgnore]
        public byte BonusHit { get; set; }
        [JsonIgnore]
        public byte BonusDmg { get; set; }
        [JsonIgnore]
        public int BonusHp { get; set; }
        [JsonIgnore]
        public int BonusMp { get; set; }
        #endregion

        [JsonIgnore]
        public Sprite Target { get; set; }

        #region Status
        [JsonIgnore]
        public bool IsSleeping => HasDebuff("sleep");
        [JsonIgnore]
        public bool IsFrozen   => HasDebuff("frozen");
        [JsonIgnore]
        public bool IsPoisoned => HasDebuff("poison");
        [JsonIgnore]
        public bool IsBleeding => HasDebuff("bleeding");
        [JsonIgnore]
        public bool IsBlind    => HasDebuff("blind");
        [JsonIgnore]
        public bool IsConfused => HasDebuff("confused");
        #endregion

        public bool HasBuff(string buff)
        {
            if (Buffs == null || Buffs.Count == 0)
                return false;

            return Buffs.Any(i => i.Has(buff));
        }

        public bool HasDebuff(string debuff)
        {
            if (Debuffs == null || Debuffs.Count == 0)
                return false;

            return Debuffs.Any(i => i.Has(debuff));
        }

        public bool RemoveBuff(string buff)
        {
            if (HasBuff(buff))
            {
                var idx     = Buffs.FindIndex(i => i.Has(buff));
                var buffobj = Buffs[idx];

                lock (Buffs)
                    buffobj.OnEnded(this, buffobj);

                return true;
            }
            return false;
        }

        public bool RemoveDebuff(string debuff)
        {
            if (HasDebuff(debuff))
            {
                var idx     = Debuffs.FindIndex(i => i.Has(debuff));
                var buffobj = Debuffs[idx];

                lock (Debuffs)
                    buffobj.OnEnded(this, buffobj);

                return true;
            }
            return false;
        }

        public void RemoveAllBuffs()
        {
            for (int i = 0; i < Buffs.Count; i++)
                RemoveBuff(Buffs[i].Name);
        }

        public void RemoveAllDebuffs()
        {
            for (int i = 0; i < Debuffs.Count; i++)
                RemoveDebuff(Debuffs[i].Name);
        }

        public void RemoveBuffsAndDebuffs()
        {
            RemoveAllBuffs();
            RemoveAllDebuffs();
        }

        public Sprite()
        {
            if (this is Aisling)
                Content = TileContent.Aisling;
            if (this is Monster)
                Content = TileContent.Monster;
            if (this is Mundane)
                Content = TileContent.Mundane;
            if (this is Money)
                Content = TileContent.Money;
            if (this is Item)
                Content = TileContent.Item;

            Buffs   = new List<Buff>();
            Debuffs = new List<Debuff>();
        }

        public Random rnd = new Random();

        public void ApplyDamage(Sprite Source, int dmg, bool truedamage = false, byte sound = 1,
            Action<int> dmgcb = null)
        {
            if (!Attackable)
                return;

            if (!CanBeAttackedHere(Source))
                return;


            if (Source is Aisling)
            {
                var client = (Source as Aisling);
                if (client.EquipmentManager.Weapon != null
                    && client.EquipmentManager.Weapon.Item != null && client.Weapon > 0)
                {
                    var weapon = client.EquipmentManager.Weapon.Item;

                    dmg += rnd.Next(weapon.Template.DmgMin + 1, weapon.Template.DmgMax + 5) * client.BonusDmg;
                }
            }

            if (this is Aisling)
            {
                var client = (this as Aisling);
                if (client != null && (client.DamageCounter++ % 2) == 0 && dmg > 0)
                {
                    client.EquipmentManager.DecreaseDurability();
                }
            }


            if (truedamage)
            {
                CurrentHp -= dmg;

                if (CurrentHp < 0)
                    CurrentHp = 0;
            }
            else
            {
                Target = Source;

                if (HasBuff("dion"))
                {
                    var empty = new ServerFormat13
                    {
                        Serial = this.Serial,
                        Health = byte.MaxValue,
                        Sound = sound
                    };

                    this.Show(Scope.NearbyAislings, empty);
                }
                else
                {

                    if (HasDebuff("sleep"))
                        dmg *= 2;

                    RemoveDebuff("sleep");

                    var amplifier = ElementManager.ElementTable[
                        (int) Source.OffenseElement,
                        (int) DefenseElement];

                    Console.WriteLine(amplifier);

                    dmg = ComputeDmgFromAc(dmg);


                    if (dmg <= 0)
                        dmg = 1;

                    if (CurrentHp > MaximumHp)
                        CurrentHp = MaximumHp;

                    var dealth = (int) (dmg / amplifier);

                    CurrentHp -= dealth;

                    if (CurrentHp < 0)
                        CurrentHp = 0;

                    dmgcb?.Invoke(dealth);

                    var hpbar = new ServerFormat13
                    {
                        Serial = Serial,
                        Health = (ushort) ((double) 100 * this.CurrentHp / (double) this.MaximumHp),
                        Sound = sound
                    };

                    //send hpbar to client
                    this.Show(Scope.NearbyAislings, hpbar);
                }
            }

            (this as Aisling)?.Client.SendStats(StatusFlags.StructB);
            (Source as Aisling)?.Client.SendStats(StatusFlags.StructB);
        }

        /// <summary>
        /// Checks the source of damage and if it's a player, check if the target is a player.
        /// is true, checks weather or not damage can be applied on the map they are on both on.
        /// </summary>
        /// <param name="Source">Player applying damage.</param>
        /// <returns>true : false</returns>
        public bool CanBeAttackedHere(Sprite Source)
        {
            if (Source is Aisling && this is Aisling)
            {
                if (CurrentMapId > 0 && ServerContext.GlobalMapCache.ContainsKey(CurrentMapId))
                {
                    if (!ServerContext.GlobalMapCache[CurrentMapId].Flags.HasFlag(MapFlags.PlayerKill))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sends Format With Target Scope.
        /// </summary>
        /// 
        public void Show<T>(Scope op, T format, Sprite[] definer = null) where T : NetworkFormat
        {
            switch (op)
            {
                case Scope.Self:
                    Client.Send(format);
                    break;
                case Scope.NearbyAislingsExludingSelf:
                    foreach (var gc in GetObjects<Aisling>(that => this.WithinRangeOf(that)))
                    {
                        if (gc.Serial != this.Serial)
                        {
                            if (this is Aisling)
                            {
                                if (!gc.Client.CanSeeHidden() && (this as Aisling).Invisible)
                                {
                                    if (format is ServerFormat33)
                                        return;
                                }

                                if (!gc.Client.CanSeeGhosts() && (this as Aisling).Dead)
                                {
                                    if (format is ServerFormat33)
                                        return;
                                }
                            }

                            gc.Client.Send(format);
                        }
                    }
                    break;
                case Scope.NearbyAislings:
                    foreach (var gc in GetObjects<Aisling>(that => this.WithinRangeOf(that)))
                    {
                        if (this is Aisling)
                        {
                            if (!gc.Client.CanSeeHidden() && (this as Aisling).Invisible)
                            {
                                if (format is ServerFormat33)
                                    return;
                            }

                            if (!gc.Client.CanSeeGhosts() && (this as Aisling).Dead)
                            {
                                if (format is ServerFormat33)
                                    return;
                            }
                        }

                        gc.Client.Send(format);
                    }
                    break;
                case Scope.VeryNearbyAislings:
                    foreach (var gc in GetObjects<Aisling>(that => this.WithinRangeOf(that, ServerContext.Config.VeryNearByProximity)))
                    {
                        if (this is Aisling)
                        {
                            if (!gc.Client.CanSeeHidden() && (this as Aisling).Invisible)
                            {
                                if (format is ServerFormat33)
                                    return;
                            }

                            if (!gc.Client.CanSeeGhosts() && (this as Aisling).Dead)
                            {
                                if (format is ServerFormat33)
                                    return;
                            }
                        }

                        gc.Client.Send(format);
                    }
                    break;
                case Scope.AislingsOnSameMap:
                    foreach (var gc in GetObjects<Aisling>(that => this.CurrentMapId == that.CurrentMapId))
                    {
                        if (this is Aisling)
                        {
                            if (!gc.Client.CanSeeHidden() && (this as Aisling).Invisible)
                            {
                                if (format is ServerFormat33)
                                    return;
                            }

                            if (!gc.Client.CanSeeGhosts() && (this as Aisling).Dead)
                            {
                                if (format is ServerFormat33)
                                    return;
                            }
                        }

                        gc.Client.Send(format);
                    }
                    break;
                case Scope.DefinedAislings:
                    if (definer != null && definer.Length > 0)
                    {
                        foreach (var gc in definer)
                        {
                            if (this is Aisling)
                            {
                                if (!gc.Client.CanSeeHidden() && (this as Aisling).Invisible)
                                {
                                    if (format is ServerFormat33)
                                        return;
                                }

                                if (!gc.Client.CanSeeGhosts() && (this as Aisling).Dead)
                                {
                                    if (format is ServerFormat33)
                                        return;
                                }
                            }

                            (gc as Aisling).Client.Send(format);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Formula : =B2 + (B2 * 10 / (B2 * 1 / A2))
        /// </summary>
        private int ComputeDmgFromAc(int dmg)
        {
            if (dmg <= 0)
                dmg = 5;

            var armor = Ac != 0 ? Ac : 1;
            var dealt  = dmg;
            var newdmg = 0;

            checked
            {
               newdmg = dealt + (int)(dmg * 1 / ((dmg * 0.5) / armor));
            }

            return newdmg;
        }


        [JsonIgnore]
        public Position Position => new Position(X, Y);

        [JsonIgnore]
        public bool Attackable => this is Monster || this is Aisling || this is Mundane;

        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        public DateTime LastUpdated { get; set; }

        public Sprite GetSprite(int x, int y) => GetObject(i => i.X == x && i.Y == y, Get.All);

        public List<Sprite> GetInfront(Sprite sprite, int tileCount = 1)
        {
            return _GetInfront(tileCount).Where(i => i != null && i.Serial != sprite.Serial).ToList();
        }

        public List<Sprite> GetInfront(int tileCount = 1)
        {
            if (this is Aisling)
            {
                return _GetInfront(tileCount).Intersect((this as Aisling).ViewFrustrum).ToList();
            }

            return _GetInfront(tileCount).ToList();
        }


        private IEnumerable<Sprite> _GetInfront(int tileCount = 1)
        {
            for (int i = 1; i <= tileCount; i++)
            {
                switch (this.Direction)
                {
                    case 0: yield return GetSprite(this.X, this.Y - i); break;
                    case 1: yield return GetSprite(this.X + i, this.Y); break;
                    case 2: yield return GetSprite(this.X, this.Y + i); break;
                    case 3: yield return GetSprite(this.X - i, this.Y); break;
                }
            }
        }


        public void Attack(Sprite _obj)
        {
            if (!CanUpdate())
                return;

            //Formula: = H6 + I6 + J6 + K6 + L6 * 10
            var dmg = 0;

            if (this is Monster || this is Mundane)
            {
                if (this is Monster)
                {
                    var obj = this as Monster;

                    _Str = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Int = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Wis = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Con = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Dex = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);

                    dmg = obj.Template.Level * _Str + _Int + _Dex + _Con + _Wis * (int)(ServerContext.Config.MonsterDamageMultipler * obj.Template.Exponent);
                    _obj.ApplyDamage(this, dmg, false, 1, (applied) => { });
                }
                else if (this is Mundane)
                {
                    var obj = this as Mundane;

                    _Str = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Int = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Wis = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Con = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);
                    _Dex = (byte)(int)(obj.Template.Level * ServerContext.Config.MonsterDamageFactor);

                    dmg = obj.Template.Level * Str + Int + Dex + Con + Wis * ServerContext.Config.MonsterDamageMultipler;
                    _obj.ApplyDamage(this, dmg, false, 1);
                }
            }

            var target = _obj;

            var action = new ServerFormat1A
            {
                Serial = this.Serial,
                Number = 0x01,
                Speed = 20
            };

            if (target.CurrentHp > 0)
            {

                var hpbar = new ServerFormat13
                {
                    Serial = target.Serial,
                    Health = (ushort)(100 * target.CurrentHp / target.MaximumHp),
                    Sound = 1
                };

                var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(this));

                foreach (var aisling in nearby)
                {
                    aisling.Show(Scope.Self, action);
                    aisling.Show(Scope.Self, hpbar);
                }
            }
        }

        public void RemoveFrom(Aisling nearbyAisling)
        {
            if (nearbyAisling != null)
            {
                nearbyAisling.Show(Scope.Self, new ServerFormat0E(this.Serial));
            }
        }

        public void ShowTo(Aisling nearbyAisling)
        {
            if (nearbyAisling != null)
            {
                nearbyAisling.Show(Scope.Self, new ServerFormat07(new Sprite[] { this }));
            }
        }

        public bool WithinRangeOf(int x, int y, int distance)
        {
            var other = new Aisling();
            other.X = x;
            other.Y = y;
            other.CurrentMapId = CurrentMapId;
            return WithinRangeOf(other, distance);
        }

        public bool WithinRangeOf(Sprite other)
        {
            if (other == null)
                return false;

            if (CurrentMapId != other.CurrentMapId)
                return false;

            return WithinRangeOf(other, (int)ServerContext.Config.WithinRangeProximity);
        }

        public bool WithinRangeOf(Sprite other, int distance)
        {
            if (other == null)
                return false;

            var xDist = Math.Abs(this.X - other.X);
            var yDist = Math.Abs(this.Y - other.Y);

            if (xDist > distance ||
                yDist > distance)
                return false;

            if (this.CurrentMapId != other.CurrentMapId)
                return false;

            var dist = Extensions.Sqrt((float)(Math.Pow(xDist, 2) + Math.Pow(yDist, 2)));
            return dist <= distance;
        }

        public bool WithinRangeOf(int x, int y)
        {
            var xDist = Math.Abs(this.X - x);
            var yDist = Math.Abs(this.Y - y);

            if (xDist > ServerContext.Config.WithinRangeProximity ||
                yDist > ServerContext.Config.WithinRangeProximity)
                return false;

            var dist = Extensions.Sqrt((float)(Math.Pow(xDist, 2) + Math.Pow(yDist, 2)));
            return dist <= ServerContext.Config.WithinRangeProximity;
        }


        public bool Facing(int x, int y)
        {
            switch ((Direction)this.Direction)
            {
                case Types.Direction.North:
                    return ((this.X == x) && ((this.Y - 1) == y));
                case Types.Direction.South:
                    return ((this.X == x) && ((this.Y + 1) == y));
                case Types.Direction.East:
                    return (((this.X + 1) == x) && (this.Y == y));
                case Types.Direction.West:
                    return (((this.X - 1) == x) && (this.Y == y));
            }

            return false;
        }

        public bool Facing(Sprite other, out int direction)
        {
            return this.Facing(other.X, other.Y, out direction);
        }

        public bool Facing(int x, int y, out int direction)
        {
            var xDist = (x - this.X).Clamp(-1, +1);
            var yDist = (y - this.Y).Clamp(-1, +1);

            direction = facingTable[xDist + 1][yDist + 1];
            return (this.Direction == direction);
        }


        public void Remove()
        {
            if (this is Monster)
                Remove<Monster>();

            if (this is Aisling)
                Remove<Aisling>();

            if (this is Money)
                Remove<Money>();

            if (this is Item)
                Remove<Item>();

            if (this is Mundane)
                Remove<Mundane>();
        }

        public Aisling[] AislingsNearby() => GetObjects<Aisling>(i => i.WithinRangeOf(this));
        public Monster[] MonstersNearby() => GetObjects<Monster>(i => i.WithinRangeOf(this));
        public Mundane[] MundanesNearby() => GetObjects<Mundane>(i => i.WithinRangeOf(this));


        /// <summary>
        /// Use this to Remove Sprites 
        /// It will remove them from ingame to who those effected.
        /// and invoke the objectmanager.
        /// </summary>
        public void Remove<T>() where T : Sprite, new()
        {
            var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(this));
            var response = new ServerFormat0E(Serial);

            foreach (var o in nearby)
                o?.Client?.Send(response);

            if (this is Monster)
                DelObject(this as Monster);
            if (this is Aisling)
                DelObject(this as Aisling);
            if (this is Money)
                DelObject(this as Money);
            if (this is Item)
                DelObject(this as Item);
            if (this is Mundane)
                DelObject(this as Mundane);

            if (Map != null)
                Map.Tile[X, Y] = TileContent.None;
        }

        public void UpdateBuffs(TimeSpan elapsedTime)
        {
            Buff[] buff_Copy;

            lock (Buffs)
                buff_Copy = new List<Buff>(Buffs).ToArray();

            for (int i = 0; i < buff_Copy.Length; i++)
                buff_Copy[i].Update(this, elapsedTime);
        }

        public void UpdateDebuffs(TimeSpan elapsedTime)
        {
            Debuff[] debuff_Copy;

            lock (Debuffs)
                debuff_Copy = new List<Debuff>(Debuffs).ToArray();

            for (int i = 0; i < debuff_Copy.Length; i++)
                debuff_Copy[i].Update(this, elapsedTime);
        }

        /// <summary>
        /// Show all nearby aislings, this sprite has turned.
        /// </summary>
        public virtual void Turn()
        {
            if (!CanUpdate())
                return;

            var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(this));

            foreach (var o in nearby)
            {
                o?.Client?.Send(new ServerFormat11
                {
                    Direction = this.Direction,
                    Serial = this.Serial,
                });
            }

            if (this is Aisling)
                SaveObject<Aisling>(this as Aisling);
        }

        public void WalkTo(int x, int y)
        {
            if (!CanUpdate())
                return;

            try
            {
                var buffer = new byte[2];
                var length = float.PositiveInfinity;
                var offset = 0;

                for (byte i = 0; i < 4; i++)
                {
                    var newX = (X + Directions[i][0]);
                    var newY = (Y + Directions[i][1]);

                    if (newX == x &&
                        newY == y)
                        continue;

                    if (Map.IsWall(this, newX, newY))
                        continue;

                    var xDist = (x - newX);
                    var yDist = (y - newY);
                    var tDist = Extensions.Sqrt(xDist * xDist + yDist * yDist);

                    if (length < tDist)
                        continue;

                    if (length > tDist)
                    {
                        length = tDist;
                        offset = 0;
                    }


                    buffer[offset.Clamp(0, buffer.Length)] = i;
                    offset++;
                }

                if (offset == 0)
                    return;

                lock (Generator.Random)
                {
                    Direction = buffer[Generator.Random.Next(0, offset)];
                }

                if (!Walk())
                    return;

                if (this is Monster)
                    SaveObject(this as Monster);

                if (this is Mundane)
                    SaveObject(this as Mundane);
            }
            catch
            {
                // ignored
            }
        }

        public virtual void Wander()
        {
            if (!CanUpdate())
                return;

            lock (Generator.Random)
            {
                Direction = (byte)Generator.Random.Next(0, 4);
            }

            if (Walk())
            {
                if (this is Monster)
                    SaveObject(this as Monster);
                if (this is Mundane)
                    SaveObject(this as Mundane);
            }
        }

        public bool CanUpdate()
        {
            if (IsSleeping || IsFrozen)
                return false;

            if (this is Monster || this is Mundane)
                if (CurrentHp == 0)
                    return false;


            return true;
        }

        public virtual bool Walk()
        {
            if (!CanUpdate())
                return false;

            var savedX = X;
            var savedY = Y;
            var savedDir = Direction;

            if (this is Aisling)
            {
                
            }


            if (Direction == 0)
            {
                if (Map.IsWall(this, X, Y - 1))
                {
                    if (this is Aisling)
                    {
                        var obj = GetObject<Aisling>(i => i.X == X && i.Y == Y - 1);
                        if (obj != null && obj.Dead)
                        {
                            Y--;
                            goto step;
                        }
                        else if (obj == null)
                        {
                            Y--;
                            goto step;

                        }
                    }
                    return false;
                }
                Y--;
            }

            if (Direction == 1)
            {
                if (Map.IsWall(this, X + 1, Y))
                {
                    if (this is Aisling)
                    {
                        var obj = GetObject<Aisling>(i => i.X == X + 1 && i.Y == Y);
                        if (obj != null && obj.Dead)
                        {
                            X++;
                            goto step;
                        }
                        else if (obj == null)
                        {
                            X++;
                            goto step;

                        }
                    }
                    return false;
                }
                X++;
            }

            if (Direction == 2)
            {
                if (Map.IsWall(this, X, Y + 1))
                {
                    if (this is Aisling)
                    {
                        var obj = GetObject<Aisling>(i => i.X == X && i.Y == Y + 1);
                        if (obj != null && obj.Dead)
                        {
                            Y++;
                            goto step;
                        }
                        else if (obj == null)
                        {
                            Y++;
                            goto step;

                        }
                    }
                    return false;
                }
                Y++;
            }

            if (Direction == 3)
            {
                if (Map.IsWall(this, X - 1, Y))
                {
                    if (this is Aisling)
                    {
                        var obj = GetObject<Aisling>(i => i.X == X - 1 && i.Y == Y);
                        if (obj != null && obj.Dead)
                        {
                            X--;
                            goto step;
                        }
                        else if (obj == null)
                        {
                            X--;
                            goto step;

                        }
                    }
                    return false;
                }
                X--;
            }

            @step:
            X = X.Clamp(X, Map.Cols - 1);
            Y = Y.Clamp(Y, Map.Rows - 1);

            CompleteWalk(savedX, savedY);

            return true;
        }

        private void CompleteWalk(int savedX, int savedY)
        {
            Map.Tile[savedX, savedY] = TileContent.None;
            Map.Tile[X, Y] = Content;

            if (this is Aisling)
            {
                Client.Send(new ServerFormat0B()
                {
                    Direction = Direction,
                    LastX = (ushort)savedX,
                    LastY = (ushort)savedY,
                });

                Client.Send(new ServerFormat32());
                SaveObject<Aisling>(this as Aisling);
            }

            //create format to send to all nearby users.
            var response = new ServerFormat0C()
            {
                Direction = this.Direction,
                Serial = Serial,
                X = (short)savedX,
                Y = (short)savedY,
            };

            if (this is Monster)
            {
                var nearby = (GetObjects<Aisling>(i => i.WithinRangeOf(this) && i.InsideView(this)));
                if (nearby.Length > 0)
                {
                    foreach (var obj in nearby)
                    {
                        obj.Show(Scope.Self, response, nearby);
                    }
                }
                SaveObject<Monster>(this as Monster);
            }

            if (this is Mundane)
            {
                var nearby = (GetObjects<Aisling>(i => i.WithinRangeOf(this) && i.InsideView(this)));
                if (nearby.Length > 0)
                {
                    foreach (var obj in nearby)
                    {
                        obj.Show(Scope.Self, response, nearby);
                    }
                }
                SaveObject<Mundane>(this as Mundane);
            }


            if (this is Aisling)
            {
                Client.Aisling.Show(this is Aisling
                    ? Scope.NearbyAislingsExludingSelf
                    : Scope.NearbyAislings, response);

                SaveObject<Aisling>(this as Aisling);
            }
        }
    }
}