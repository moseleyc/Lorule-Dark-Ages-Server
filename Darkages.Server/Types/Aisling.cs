using Darkages.Common;
using Darkages.Network;
using Darkages.Network.ServerFormats;
using Darkages.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Darkages
{
    public class Aisling : Sprite
    {
        public int AbpLimit { get; set; }
        public int ExpLimit { get; set; }
        public int CurrentWeight { get; set; }
        public int MaximumWeight { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastLogged { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int AbpLevel { get; set; }
        public int AbpTotal { get; set; }
        public int AbpNext { get; set; }
        public int ExpLevel { get; set; }
        public int ExpTotal { get; set; }
        public int ExpNext { get; set; }
        public Gender Gender { get; set; }
        public int Title { get; set; }
        public AislingFlags Flags { get; set; }
        public int AreaID { get; set; }
        public int ClassID { get; set; }
        public int GamePoints { get; set; }
        public int GoldPoints { get; set; }
        public int StatPoints { get; set; }
        public int Medal { get; set; }
        public int LIcon { get; set; }
        public int RIcon { get; set; }
        public int BodyColor { get; set; }
        public int BodyStyle { get; set; }
        public int FaceColor { get; set; }
        public int FaceStyle { get; set; }
        public int HairColor { get; set; }
        public int HairStyle { get; set; }
        public byte Boots { get; set; }
        public int Helmet { get; set; }
        public byte Shield { get; set; }
        public ushort Weapon { get; set; }
        public ushort Armor { get; set; }

        public GroupStatus PartyStatus { get; set; }

        public Party GroupParty { get; set; }

        public SkillBook SkillBook { get; set; }

        public SpellBook SpellBook { get; set; }

        public Inventory Inventory { get; set; }

        public EquipmentManager EquipmentManager { get; set; }

        public ActivityStatus ActiveStatus { get; set; }

        [JsonIgnore]
        public bool Dead => Flags == AislingFlags.Dead;

        [JsonIgnore]
        public bool Invisible => Flags == AislingFlags.Invisible;

        public byte Blind { get; set; }

        [JsonIgnore]
        public List<Sprite> ViewFrustrum = new List<Sprite>();

        public byte HeadAccessory1 { get; set; }

        public byte HeadAccessory2 { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public new Position Position => new Position(X, Y);

        public ClassStage Stage { get; set; }
        public Class Path { get; set; }

        [Browsable(false)]
        public Legend LegendBook { get; set; }

        public BodySprite Display { get; set; }
        public string ClanTitle { get; set; }
        public string ClanRank { get; set; }

        [JsonIgnore]
        public CastInfo ActiveSpellInfo { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public bool IsCastingSpell { get; set; }
        public byte OverCoat { get; set; }
        public byte Pants { get; set; }
        public byte[] PictureData { get; set; }
        public string ProfileMessage { get;  set; }
        public bool LoggedIn { get; set; }
        public byte Nation { get; set; }
        public string Clan { get; set; }

        [JsonIgnore] public int DamageCounter = 0;

        public List<Quest> Quests = new List<Quest>();


        public Aisling()
        {
            OffenseElement = ElementManager.Element.None;
            DefenseElement = ElementManager.Element.None;
            Clan = "";
            Nation = 3;
            Flags = AislingFlags.Normal;
            GroupParty = new Party();
            LegendBook = new Legend();
            ClanTitle = string.Empty;
            ClanRank = string.Empty;
            ActiveSpellInfo = null;
            LoggedIn = false;
            ActiveStatus = ActivityStatus.Awake;
        }

        public bool InsideView(Sprite obj)
        {
            try
            {
                List<Sprite> _view;

                lock (ViewFrustrum)
                {
                    _view = new List<Sprite>(ViewFrustrum);


                    for (int i = 0; i < _view.Count; i++)
                    {
                        if (_view[i] != null)
                        {
                            if (obj.Serial == _view[i].Serial)
                                return true;
                        }
                    }
                }
            }
            catch
            {
                obj.Remove<Monster>();
                return false;
            }

            return false;
        }

        public void RemoveFromView(Sprite obj)
        {
            lock (ViewFrustrum)
            {
                if (ViewFrustrum.Count == 0)
                    return;

                if (ViewFrustrum.Contains(obj))
                    ViewFrustrum.Remove(obj);
            }
        }

        public void View(Sprite obj)
        {
            if (!InsideView(obj))
            {
                ViewFrustrum.Add(obj);
            }
        }

        public void CastSpell(Spell spell)
        {
            if (!spell.CanUse())
            {
                spell.InUse = false;
                return;
            }

            if (spell.InUse)
                return;

            var info = Client.Aisling.ActiveSpellInfo;

            if (info != null)
            {
                var target = GetObject(i => i.Serial == info.Target, Get.Monsters | Get.Aislings | Get.Mundanes);
                spell.InUse = true;

                if (target != null)
                {
                    if (target is Aisling)
                        spell.Script.OnUse(this, target as Aisling);
                    if (target is Monster)
                        spell.Script.OnUse(this, target as Monster);
                    if (target is Mundane)
                        spell.Script.OnUse(this, target as Mundane);
                }
                else
                    spell.Script.OnUse(this, this);
            }
            else
            {
                spell.Script.OnUse(this, this);
            }
            spell.NextAvailableUse = DateTime.UtcNow.AddSeconds(0.5);
            spell.InUse = false;
        }

        public void RemoveFromNearby()
        {
            Show(Scope.NearbyAislings, new ServerFormat0E(this.Serial));
        }

        public static Aisling Create()
        {
            var result = new Aisling()
            {
                Created = DateTime.UtcNow,
                LastLogged = DateTime.UtcNow,

                Username = string.Empty,
                Password = string.Empty,
                X = 28,
                Y = 29,

                AbpLevel = 0,
                AbpTotal = 0,
                AbpNext = 0,

                ExpLevel = 1,
                ExpTotal = 1,
                ExpNext = 600,
                Gender = 0,
                Title = 0,
                Flags = 0,
                AreaID = ServerContext.Config.StartingMap,
                ClassID = 0,
                Stage = ClassStage.Class,
                Path = Class.Peasant,
                CurrentHp  = 60,
                CurrentMp  = 30,
                _MaximumHp = 60,
                _MaximumMp = 30,
                _Ac = ServerContext.Config.BaseAC,
                _Str = 3,
                _Int = 3,
                _Wis = 3,
                _Con = 3,
                _Dex = 3,

                GamePoints = 0,
                GoldPoints = 1000,
                StatPoints = 0,

                Medal = 0,
                LIcon = 0,
                RIcon = 0,

                BodyColor = 0,
                BodyStyle = 0,
                FaceColor = 0,
                FaceStyle = 0,
                HairColor = 0,
                HairStyle = 0,
                CurrentMapId = ServerContext.Config.StartingMap,
                SkillBook = new SkillBook(),
                SpellBook = new SpellBook(),
                Inventory = new Inventory(),
                EquipmentManager = new EquipmentManager(null),
            };

            foreach (var skill in ServerContext.GlobalSkillTemplateCache.Keys)
                Skill.GiveTo(result, skill);

            foreach (var spell in ServerContext.GlobalSpellTemplateCache.Keys)
                Spell.GiveTo(result, spell);

            result.LegendBook.AddLegend(new Legend.LegendItem()
            {
                Category = "Event",
                Color = (byte)LegendColor.Green,
                Icon = (byte)LegendIcon.Victory,
                Value = string.Format("Lorule: VIP {0}", DateTime.UtcNow.ToShortDateString())
            });

            if (ServerContext.GlobalMapCache.ContainsKey(result.AreaID))
            {
                result.Map = ServerContext.GlobalMapCache[509];
            }

            return result;
        }

        /// <summary>
        /// Removes Aisling, Sends Remove packet to nearby aislings
        /// and removes itself from the ObjectManager.
        /// </summary>
        public void Remove(bool update = false, bool delete = true)
        {
            if (Map != null)
            {
                Map.Tile[X, Y] = TileContent.None;
            }

            if (update)
            {
                Show(Scope.NearbyAislingsExludingSelf, new ServerFormat0E(this.Serial));
            }

            if (delete)
            {
                DelObject<Aisling>(this);
            }
        }

        internal bool HasSkill<T>(SkillScope scope) where T : Template, new()
        {
            var obj = new T();

            if (obj is SkillTemplate)
            {
                if ((scope & SkillScope.Assail) == SkillScope.Assail)
                {
                    return SkillBook.Get(i => i != null && i.Template != null
                            && i.Template.Type == SkillScope.Assail).Length > 0;
                }
            }

            return false;
        }


        /// <summary>
        /// This FUnction will return all skills that are assail-like, Assail, Clobber, Ect.
        /// </summary>
        internal Skill[] GetAssails(SkillScope scope)
        {
            if ((scope & SkillScope.Assail) == SkillScope.Assail)
            {
                return SkillBook.Get(i => i != null && i.Template != null
                        && i.Template.Type == SkillScope.Assail).ToArray();
            }

            return null;
        }
    }
}
