﻿using Darkages.Network.ServerFormats;
using Darkages.Types;
using System;
using System.Collections.Generic;

namespace Darkages.Network.Game.Components
{
    public class ObjectComponent : GameServerComponent
    {
        private readonly GameServerTimer _updateEventScheduler;
        private readonly GameServerTimer _updateMediatorScheduler;

        public ObjectComponent(GameServer server)
            : base(server)
        {
            _updateEventScheduler =
                new GameServerTimer(TimeSpan.FromSeconds(ServerContext.Config.ObjectHandlerInterval));
            _updateMediatorScheduler =
                new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.ObjectGarbageCollectorInterval));

        }

        public void OnObjectAdded(Sprite obj)
        {
            var Map = ServerContext.GlobalMapCache[obj.CurrentMapId];
            if (Map == null)
                return;

            OnObjectUpdate(obj);

            if (obj.CurrentHp > 0)
                Map.Update(obj.X, obj.Y, obj.Content);
        }

        public void OnObjectRemoved(Sprite obj)
        {
            if (obj == null)
                return;

            if (!ServerContext.GlobalMapCache.ContainsKey(obj.CurrentMapId))
                return;

            OnObjectUpdate(obj);
            obj.Map?.Update(obj.X, obj.Y, TileContent.None);

            if (obj is Monster || obj is Mundane)
            {
                var nearByAislings = obj.GetObjects<Aisling>(i => i.WithinRangeOf(obj));
                foreach (var nearbyAisling in nearByAislings)
                {
                    if (obj is Monster)
                        if ((obj as Monster).Script != null)
                            (obj as Monster).Script.OnDeath(nearbyAisling.Client);

                    nearbyAisling.RemoveFromView(obj);
                    obj.RemoveFrom(nearbyAisling);
                }
            }
        }

        public void OnObjectUpdate(Sprite obj)
        {
            if (!ServerContext.GlobalMapCache.ContainsKey(obj.CurrentMapId))
                return;

            UpdateOutOfRangeObjects(obj);
            UpdateMonsterViewFrustrum(obj);
        }

        private static void UpdateOutOfRangeObjects(Sprite obj)
        {
            var distantObjs = obj.GetObjects(i => !i.WithinRangeOf(obj)
                                                  && obj.CurrentMapId == i.CurrentMapId,
                Get.Aislings | Get.Monsters | Get.Mundanes | Get.Items | Get.Money);

            foreach (var dObj in distantObjs)
            {
                if (obj is Aisling)
                    if ((obj as Aisling).InsideView(dObj))
                    {
                        (obj as Aisling).RemoveFromView(dObj);
                        dObj.RemoveFrom(obj as Aisling);
                    }

                if (dObj is Item || dObj is Money)
                {
                    dObj.AbandonedDate = DateTime.UtcNow;
                }
            }
        }

        private static void UpdateMonsterViewFrustrum(Sprite obj)
        {
            //get aislings in range of this object that is not already.
            var nearByAislings = obj.GetObjects<Aisling>(i => i.WithinRangeOf(obj));

            foreach (var nearbyAisling in nearByAislings)
            {
                //aisling already has seen this object before, check if it is out of view range
                if (nearbyAisling.InsideView(obj) && !nearbyAisling.WithinRangeOf(obj))
                {
                    //monster has been seen before, but is now out of view
                    //so we must remove it from his view.
                    nearbyAisling.RemoveFromView(obj);

                    //now tell the server to display the remove packet.
                    obj.RemoveFrom(nearbyAisling);

                    //Invoke Onleave
                    if (obj is Monster)
                        (obj as Monster).Script
                            ?.OnLeave(nearbyAisling.Client);
                }

                //aisling has not seen this object before.
                if (!nearbyAisling.InsideView(obj))
                    if (nearbyAisling.WithinRangeOf(obj))
                    {
                        //construct batch
                        var spriteBatch = new List<Sprite>();


                        //add parent to batch
                        if (!(obj is Aisling))
                        {
                            //invoke OnApproach
                            if (obj is Monster)
                                (obj as Monster).Script
                                    ?.OnApproach(nearbyAisling.Client);

                            spriteBatch.Add(obj);
                        }
                        else
                        {
                            if (obj.Client.Aisling.Serial != nearbyAisling.Client.Aisling.Serial &&
                                !(obj as Aisling).Dead)
                            {
                                if ((obj as Aisling).Flags == AislingFlags.Invisible)
                                    return;

                                nearbyAisling.Show(Scope.Self,
                                    new ServerFormat33(nearbyAisling.Client, obj as Aisling));
                            }
                        }

                        nearbyAisling.View(obj);

                        //check how much packets we need to send.
                        //this makes sure we don't overflow the client
                        //sending to much display packets at once.
                        //so we chunk them up.
                        // var blocks = Split(spriteBatch).ToArray();

                        var payLoad = new ServerFormat07(spriteBatch.ToArray());

                        nearbyAisling.Show(
                            Scope.DefinedAislings,
                            payLoad,
                            nearByAislings);

                        foreach (var block in spriteBatch)
                        {
                            nearbyAisling.View(block);
                        }
                    }
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            _updateEventScheduler.Update(elapsedTime);
            _updateMediatorScheduler.Update(elapsedTime);

            if (_updateMediatorScheduler.Elapsed)
            {
                InvokeMediators();

                _updateMediatorScheduler.Reset();
            }
        }

        public void InvokeMediators(Area area = null)
        {
            MonsterMediator(area);
            ItemMediator(area);
        }

        private void MonsterMediator(Area area = null)
        {
            var objects = area != null
                ? GetObjects(n => n != null && n.CurrentMapId == area.ID, Get.Monsters)
                : GetObjects(n => n != null, Get.Monsters);

            if (objects != null)
            {
                var c = 0;
                foreach (var obj in objects)
                    if (obj.CurrentHp == 0)
                    {
                        if (ServerContext.Config.ShowMonsterDeathAnimation)
                            obj.Show(Scope.NearbyAislings,
                                new ServerFormat29(ServerContext.Config.MonsterDeathAnimationNumber, (ushort)obj.X,
                                    (ushort)obj.Y));
                        OnObjectRemoved(obj);
                        c++;
                    }
            }
        }

        private void ItemMediator(Area area = null)
        {
            var objects = area != null
                ? GetObjects(n => n != null && n.CurrentMapId == area.ID, Get.Money | Get.Items)
                : GetObjects(n => n != null, Get.Money | Get.Items);

            var removes = 0;
            foreach (var obj in objects)
                if ((DateTime.UtcNow - obj.AbandonedDate).TotalSeconds > ServerContext.Config.DropDecayInSeconds)
                {
                    obj.Remove();
                    removes++;
                }

            if (removes > 0 && ServerContext.Config.LogDestroyedObjects)
                Console.WriteLine("[ObjectComponent] {0} Objects Destroyed. (Abandoned Item, Money.) ", removes);
        }
    }
}












