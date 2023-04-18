using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TT2Decryptor
{
    public class TT2SaveConverter
    {
        private const string SaveFileEncryptionKeyString = "4bc07927192f4e9a";

        private static JObject SaveObject { get; set; } = new JObject();
        private static readonly Dictionary<string, string> setTranslation = new();
        private static readonly Dictionary<string, string> skillTranslation = new();
        static async Task Main(string[] args) {
            Console.Write(Match());
            await MainAsync();
        }
        private static async Task MainAsync()
        {
            string decryptedJson = GetSaveFileContentString();
            await File.WriteAllTextAsync("../../../decrypted.json", decryptedJson);
            string resultJson = GetResultJson();
            await File.WriteAllTextAsync("../../../result.json", resultJson);
        }
        private static string GetResultJson()
        {           
            JObject playerStats = new()
            {
                new JProperty("Max Prestige Stage", SaveObject["PrestigeModel"]["maxPrestigeStageCount"].ToString()),
                new JProperty("Artifacts Collected", SaveObject["AccountModel"]["playerProfileData"]["artifactCount"].ToString()),
                new JProperty("Crafting Power", ((int) SaveObject["AccountModel"]["playerProfileData"]["craftingShardsSpent"] / 500).ToString()),
                new JProperty("Total Pet Levels", SaveObject["AccountModel"]["playerProfileData"]["totalPetLevels"].ToString()),
                new JProperty("Skill Points Owned", "3699"),
                new JProperty("Hero Weapon Upgrades", "1938"),
                new JProperty("Hero Scroll Upgrades", "2710"),
                new JProperty("Tournaments Joined", "300"),
                new JProperty("Undisputed Wins", "77"),
                new JProperty("Tournament Points", "43728"),
                new JProperty("Lifetime Relics", "1.256E+204")
            };
            JObject raidStats = new()
            {
                new JProperty("Raid Level", "412"),
                new JProperty("Raid Damage", "512"),
                new JProperty("Total Raid Experience", "258520"),
                new JProperty("Total Raid Attacks", "3602"),
                new JProperty("Total Raid Card Levels", "719"),
                new JProperty("Raid Cards Owned", "35"),
                new JProperty("Wildcards", "68"),
                new JProperty("Lifetime Clan Morale", "37684")
            };
            JObject splashStats = new()
            {
                new JProperty("Splash Damage", "2.251E+71"),
                new JProperty("All Splash Count", "15"),
                new JProperty("All Splash Skip", "1.245"),
                new JProperty("Heavenly Strike Splash Count", "21"),
                new JProperty("Pet Splash Count", "15"),
                new JProperty("Dual Burst Splash Count", "46"),
                new JProperty("Clan Ship Splash Count", "29"),
                new JProperty("Dagger Splash Count", "15"),
                new JProperty("Heavenly Strike Splash Skip", "274"),
                new JProperty("Pet Splash Skip", "353.7"),
                new JProperty("Clan Ship Splash Skip", "552.9"),
                new JProperty("Shadow Clone Splash Skip", "281.4"),
                new JProperty("Dagger Splash Skip", "317.5")
            };
            JObject passiveSkills = new()
            {
                new JProperty("Intimidating Presence", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["LessMonsters"]),
                new JProperty("Power Surge", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["PetSplashSkip"]),
                new JProperty("Anti-Titan Cannon", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["ClanShipSplashSkip"]),
                new JProperty("Mystical Impact", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["SorcererSplashSkip"]),
                new JProperty("Arcane Bargain", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["RaidCardPower"]),
                new JProperty("Silent March", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["SilentMarch"]),
                new JProperty("Cloak And Dagger", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["DaggerSplashSkip"]),
                new JProperty("Golden Forge", SaveObject["PassiveSkillModel"]["allPassiveSkillIdLevels"]["AlchemistSplashSkip"])
            };
            JObject json = new()
            {
                new JProperty("playerStats", playerStats),
                new JProperty("raidStats", raidStats),
                new JProperty("artifacts", BuildArtifacts()),
                new JProperty("splashStats", splashStats),
                new JProperty("raidCards", BuildRaidCards()),
                new JProperty("equipmentSets", BuildEquipmentSets()),
                new JProperty("passiveSkills", passiveSkills),
                new JProperty("petLevels", BuildPetLevels()),
                new JProperty("skillTree", BuildSkillTree())
            };
            return json.ToString();
        }
        private static JObject BuildSkillTree()
        {
            JObject skillTree = new();
            JObject idToLevel = (JObject)SaveObject["SkillTreeModel"]["idToLevelDict2.0"];
            foreach(JProperty skill in idToLevel.Children<JProperty>())
            {
                skillTree.Add(new JProperty(TranslateSkill(skill.Name), skill.Value));
            }
            return skillTree;
        }
        public static string TranslateSkill(string skill)
        {
            if(0 == skillTranslation.Count)
            {
                skillTranslation.Add("TapDmg", "Knight's Valor");
                skillTranslation.Add("TapDmgFromHelpers", "Chivalric Order");
                skillTranslation.Add("BurstSkillBoost", "Angelic Radiance");
                skillTranslation.Add("HeavyStrikes", "Cleaving Strike");
                skillTranslation.Add("BurstDamageMultiCastSkill", "Divine Wrath");
                skillTranslation.Add("FairyGold", "Will of Midas");
                skillTranslation.Add("Frenzy", "Barbaric Fury");
                skillTranslation.Add("BurstSkillMana", "Rejuvenation");
                skillTranslation.Add("FairyChance", "Fairy Charm");
                skillTranslation.Add("TwilightGathering", "Twilight Gathering");
                skillTranslation.Add("TwilightGatheringMultiCastSkill", "Eventide Afterglow");
                skillTranslation.Add("PetDmg", "Pet Evolution");
                skillTranslation.Add("FireTapSkillBoost", "Summon Inferno");
                skillTranslation.Add("PetEquipmentBoost", "Companion Warfare");
                skillTranslation.Add("PetGoldQTE", "Heart of Gold");
                skillTranslation.Add("CombatTechniques", "Combat Techniques");
                skillTranslation.Add("TapBoostMultiCastSkill", "Volcanic Eruption");
                skillTranslation.Add("PetQTE", "Lightning Burst");
                skillTranslation.Add("DualPetBoost", "Summoning Circle");
                skillTranslation.Add("PetBonusBoost", "Ember Arts");
                skillTranslation.Add("BossDmgQTE", "Flash Zip");
                skillTranslation.Add("DualPetMultiCast", "Burning Passion");
                skillTranslation.Add("AllHelperDmg", "Master Commander");
                skillTranslation.Add("HelperDmgSkillBoost", "Heroic Might");
                skillTranslation.Add("ClanShipDmg", "Aerial Assault");
                skillTranslation.Add("HelperBoost", "Tactical Insight");
                skillTranslation.Add("HelperInspiredWeaken", "Searing Light");
                skillTranslation.Add("ClanQTE", "Coordinated Offensive");
                skillTranslation.Add("HelperDmgQTE", "Astral Awakening");
                skillTranslation.Add("HelperBoostMultiCastSkill", "Command Supremacy");
                skillTranslation.Add("ClanShipStun", "Anchoring Shot");
                skillTranslation.Add("ClanShipVoltage", "Voltaic Sails");
                skillTranslation.Add("ClanShipVoltageMultiCastSkill", "Galvanized Mast");
                skillTranslation.Add("CloneDmg", "Phantom Vengeance");
                skillTranslation.Add("MPCapacityBoost", "Limit Break");
                skillTranslation.Add("CloneSkillBoost", "Eternal Darkness");
                skillTranslation.Add("BossTimer", "Dimensional Shift");
                skillTranslation.Add("ManaMonster", "Manni Mana");
                skillTranslation.Add("ShadowCloneMultiCastSkill", "Phantom Control");
                skillTranslation.Add("CritSkillBoost", "Lightning Strike");
                skillTranslation.Add("ManaStealSkillBoost", "Mana Siphon");
                skillTranslation.Add("ForbiddenContract", "Forbidden Contract");
                skillTranslation.Add("PuppetMaster", "Nightmare Puppeteer");
                skillTranslation.Add("OfflineGold", "Master Thief");
                skillTranslation.Add("CritSkillBoostDmg", "Assassinate");
                skillTranslation.Add("UltraDagger", "Summon Dagger");
                skillTranslation.Add("MultiMonsters", "Ambush");
                skillTranslation.Add("PoisonedBlade", "Poison Edge");
                skillTranslation.Add("Cloaking", "Cloaking");
                skillTranslation.Add("GuidedBlade", "Deadly Focus");
                skillTranslation.Add("CriticalHit", "Weakpoint Throw");
                skillTranslation.Add("StreamOfBladesMultiCastSkill", "Mark of Death");
                skillTranslation.Add("Backstab", "Backstab");
                skillTranslation.Add("LoadedDice", "Loaded Dice");
                skillTranslation.Add("Transmutation", "Transmutation");
                skillTranslation.Add("GoldGun", "Gold Gun");
                skillTranslation.Add("ChestGold", "Chesterson Incense");
                skillTranslation.Add("MidasSkillBoost", "Midas Ultimate");
                skillTranslation.Add("MagnumOpus", "Magnum Opus");
                skillTranslation.Add("LovePotion", "Love Potion");
                skillTranslation.Add("HandOfMidasMultiCastSkillBoost", "Midas Overflow");
                skillTranslation.Add("RoyalContract", "Royal Contract");
                skillTranslation.Add("KratosSummon", "Sprouting Salts");
                skillTranslation.Add("AuricShot", "Auric Shot");
                skillTranslation.Add("TwilightGatheringMultiCast", "SKILLTREE_TWILIGHTGATHERINGMULTICAST");
                skillTranslation.Add("ClanShipVoltageMultiCast", "SKILLTREE_CLANSHIPVOLTAGEMULTICAST");
                skillTranslation.Add("StrokeOfLuck", "Stroke of Luck");
                skillTranslation.Add("FairyGoldBoost", "SKILLTREE_FAIRYGOLDBOOST");
                skillTranslation.Add("Fairy", "SKILLTREE_FAIRY");
                skillTranslation.Add("DualPetMultiCastBoost", "SKILLTREE_DUALPETMULTICASTBOOST");
                skillTranslation.Add("None", "None");
            }
            if (!skillTranslation.TryGetValue(skill, out string value) || value.Equals(""))
            {
                value = "MISSING: " + skill;
            }
            return value;
        }
        private static JObject BuildPetLevels()
        {
            JObject petLevels = new();
            string[] pets = { "Nova", "Toto", "Cerberus", "Mousy", "Harker", "Bubbles", "Demos", "Tempest", "Basky", "Scraps", "Zero", "Polly", "Hamy", "Phobos", "Fluffers", "Kit", "Soot", "Klack", "Cooper", "Jaws", "Xander", "Griff", "Basil", "Bash", "Violet", "Annabelle", "Effie", "Percy", "Cosmos", "Taffy" };
            JObject allPetXps = (JObject)SaveObject["PetModel"]["allPetXps"];
            foreach(JProperty pet in allPetXps.Children<JProperty>())
            {
                int level = (int)pet.Value / 100;
                int.TryParse(pet.Name[3..], out int id);
                petLevels.Add(new JProperty(pets[id - 1], level));
            }
            return petLevels;
        }
        private static JArray BuildEquipmentSets()
        {
            JArray equipmentSets = new JArray();
            JArray allLookIDs = SaveObject["EquipmentModel"]
                .ToObject<JObject>()
                .GetValue("allLookIDs")
                .ToObject<JArray>();
            Dictionary<string, int> sets = new Dictionary<string, int>();
            foreach(JValue lookID in allLookIDs)
            {
                string[] id = lookID.ToString().Split('_', 2);
                if (sets.TryGetValue(id[1], out int value))
                {
                    sets[id[1]] = value + 1;
                }
                else
                {
                    sets.Add(id[1], value + 1);
                }
            }
            foreach(KeyValuePair<string,int> pair in sets)
            {
                if(5 == pair.Value)
                {
                    equipmentSets.Add(new JValue(TranslateEquipment(pair.Key)));
                }
            }
            return equipmentSets;
        }

        private static JObject BuildArtifacts()
        {
            JObject artifacts = new JObject();
            JObject allArtifacts = (JObject)SaveObject["ArtifactModel"]["allArtifactInfo"];
            foreach (JProperty property in allArtifacts.Children<JProperty>())
            {
                if (!property.Value.Type.Equals(JTokenType.Object))
                {
                    break;
                }
                JObject value = (JObject)property.Value;
                string id = property.Name[8..];
                string artifactName = "";
                switch (id)
                {
                    case "1": artifactName = "Heroic Shield"; break;
                    case "2": artifactName = "Stone of the Valrunes"; break;
                    case "3": artifactName = "The Arcana Cloak"; break;
                    case "4": artifactName = "Axe of Muerte"; break;
                    case "5": artifactName = "Invader's Shield"; break;
                    case "6": artifactName = "Elixir of Eden"; break;
                    case "7": artifactName = "Parchment of Foresight"; break;
                    case "8": artifactName = "Hunter's Ointment"; break;
                    case "9": artifactName = "Laborer's Pendant"; break;
                    case "10": artifactName = "Bringer of Ragnarok"; break;
                    case "11": artifactName = "Titan's Mask"; break;
                    case "12": artifactName = "Swamp Gauntlet"; break;
                    case "13": artifactName = "Forbidden Scroll"; break;
                    case "14": artifactName = "Aegis"; break;
                    case "15": artifactName = "Ring of Fealty"; break;
                    case "16": artifactName = "Glacial Axe"; break;
                    case "17": artifactName = "Helmet of Madness"; break;
                    case "18": artifactName = "Egg of Fortune"; break;
                    case "19": artifactName = "Chest of Contentment"; break;
                    case "20": artifactName = "Book of Prophecy"; break;
                    case "21": artifactName = "Divine Chalice"; break;
                    case "22": artifactName = "Book of Shadows"; break;
                    case "23": artifactName = "Titanium Plating"; break;
                    case "24": artifactName = "Staff of Radiance"; break;
                    case "25": artifactName = "Blade of Damocles"; break;
                    case "26": artifactName = "Heavenly Sword"; break;
                    case "27": artifactName = "Glove of Kuma"; break;
                    case "28": artifactName = "Amethyst Staff"; break;
                    case "29": artifactName = "Drunken Hammer"; break;
                    case "30": artifactName = "Influential Elixir"; break;
                    case "31": artifactName = "Divine Retribution"; break;
                    case "32": artifactName = "The Sword of Storms"; break;
                    case "33": artifactName = "Furies Bow"; break;
                    case "34": artifactName = "Charm of the Ancient"; break;
                    case "35": artifactName = "Hero's Blade"; break;
                    case "36": artifactName = "Infinity Pendulum"; break;
                    case "37": artifactName = "Oak Staff"; break;
                    case "38": artifactName = "Fruit of Eden"; break;
                    case "39": artifactName = "Titan Spear"; break;
                    case "40": artifactName = "Ring of Calisto"; break;
                    case "41": artifactName = "Royal Toxin"; break;
                    case "42": artifactName = "Avian Feather"; break;
                    case "43": artifactName = "Zakynthos Coin"; break;
                    case "44": artifactName = "Great Fay Medallion"; break;
                    case "45": artifactName = "Neko Sculpture"; break;
                    case "46": artifactName = "Corrupted Rune Heart"; break;
                    case "47": artifactName = "Invader's Gjallarhorn"; break;
                    case "48": artifactName = "Phantom Timepiece"; break;
                    case "49": artifactName = "The Master's Sword"; break;
                    case "50": artifactName = "Ambrosia Elixir"; break;
                    case "51": artifactName = "Samosek Sword"; break;
                    case "52": artifactName = "Heart of Storms"; break;
                    case "53": artifactName = "Apollo Orb"; break;
                    case "54": artifactName = "Essence of the Kitsune"; break;
                    case "55": artifactName = "Durendal Sword"; break;
                    case "56": artifactName = "Helheim Skull"; break;
                    case "57": artifactName = "Aram Spear"; break;
                    case "58": artifactName = "Mystic Staff"; break;
                    case "59": artifactName = "The Retaliator"; break;
                    case "60": artifactName = "Ward of the Darkness"; break;
                    case "61": artifactName = "Tiny Titan Tree"; break;
                    case "62": artifactName = "Helm of Hermes"; break;
                    case "63": artifactName = "Lost King's Mask"; break;
                    case "64": artifactName = "O'Ryan's Charm"; break;
                    case "65": artifactName = "Hourglass of the Impatient"; break;
                    case "66": artifactName = "Khrysos Bowl"; break;
                    case "67": artifactName = "Earrings of Portara"; break;
                    case "68": artifactName = "Mystical Beans of Senzu"; break;
                    case "69": artifactName = "Lucky Foot of Al-mi'raj"; break;
                    case "70": artifactName = "Boots of Hermes"; break;
                    case "71": artifactName = "Morgelai Sword"; break;
                    case "72": artifactName = "Oberon Pendant"; break;
                    case "73": artifactName = "Moonlight Bracelet"; break;
                    case "74": artifactName = "Unbound Gauntlet"; break;
                    case "75": artifactName = "Oath's Burden"; break;
                    case "76": artifactName = "Crown of the Constellation"; break;
                    case "77": artifactName = "Titania's Sceptre"; break;
                    case "78": artifactName = "Fagin's Grip"; break;
                    case "79": artifactName = "Coins of Ebizu"; break;
                    case "80": artifactName = "The Magnifier"; break;
                    case "81": artifactName = "The Treasure of Fergus"; break;
                    case "82": artifactName = "The Bronzed Compass"; break;
                    case "83": artifactName = "Stryfe's Peace"; break;
                    case "84": artifactName = "Flute of the Soloist"; break;
                    case "85": artifactName = "The White Dwarf"; break;
                    case "86": artifactName = "Sword of the Royals"; break;
                    case "87": artifactName = "Spearit's Vigil"; break;
                    case "88": artifactName = "The Cobalt Plate"; break;
                    case "89": artifactName = "Sigils of Judgement"; break;
                    case "90": artifactName = "Foliage of the Keeper"; break;
                    case "91": artifactName = "Ringing Stone"; break;
                    case "92": artifactName = "Quill of Scrolls"; break;
                    case "93": artifactName = "Old King's Stamp"; break;
                    case "94": artifactName = "Evergrowing Stack"; break;
                    case "95": artifactName = "Charged Card"; break;
                    case "96": artifactName = "Hades Orb"; break;
                    case "97": artifactName = "Sticky Fruit"; break;
                    case "98": artifactName = "Shimmering Oil"; break;
                    case "99": artifactName = "Golden Scope"; break;
                    case "100": artifactName = "Twin Bracers"; break;
                    case "101": artifactName = "Cosmic Sextant"; break;
                    case "102": artifactName = "Endless Bandolier"; break;
                    case "103": artifactName = "Pearl of Oblivion"; break;
                    default: break;
                }

                bool enchanted = (int)value.GetValue("enchantment_level") > 0;

                JObject levelObject = (JObject)value.GetValue("level");
                string level;
                string significand = levelObject.GetValue("significand").ToString();
                if (significand.Length <= 5)
                {
                    int diff = 4 - significand.Length;
                    if (significand.Contains('.'))
                    {
                        level = significand + "0";
                    }
                    else
                    {
                        level = significand + ".";
                    }
                    
                    for (int i = 0; i < diff; i++)
                    {
                        level += "0";
                    }

                }
                else
                {
                    level = significand[..5];
                }
                level = level + "E+" + levelObject.GetValue("exponent").ToString().Split('.')[0];

                JObject artifactDetail = new()
                {
                    new JProperty("enchanted", enchanted.ToString()),
                    new JProperty("level", level)
                };

                JProperty artifact = new(artifactName, artifactDetail);
                artifacts.Add(artifact);
            }
            return artifacts;
        }

        private static JObject BuildRaidCards()
        {
            JObject raidCards = new JObject();
            JArray allCards = (JArray)SaveObject["RaidCardModel"]["cards"];
            foreach (JObject card in allCards)
            {
                string skill_name = (string)(JValue)card.GetValue("skill_name");
                int level = (int)(JValue)card.GetValue("level");
                int recieved = (int)(JValue)card.GetValue("quantity_received");
                int spent = (int)(JValue)card.GetValue("quantity_spent");

                string name = Regex.Replace(skill_name, "(?<=[a-z])([A-Z])", " $1");
                int cards = recieved - spent;
                raidCards.Add(
                    new JProperty(
                        name,
                        new JObject {
                            new JProperty("level",level),
                            new JProperty("cards",cards)
                        }
                    )
                );
            }
            return raidCards;
        }
        private static string TranslateEquipment(string equipment)
        {
            if(0 == setTranslation.Count)
            {
                setTranslation.Add("FireKnight", "Ignus, the Volcanic Phoenix");
                setTranslation.Add("WaterSorcerer", "Kor, the Whispering Wave");
                setTranslation.Add("EarthRogue", "Styxsis, the Single Touch");
                setTranslation.Add("ElectricWarlord", "Ironheart, the Crackling Tiger");
                setTranslation.Add("Mech", "Mechanized Sword");
                setTranslation.Add("Samurai", "Fatal Samurai");
                setTranslation.Add("HighTecLightning", "Angelic Guardian");
                setTranslation.Add("Skulls", "Ruthless Necromancer");
                setTranslation.Add("Pirate", "Treasure Hunter");
                setTranslation.Add("BoneTribe", "Ancient Warrior");
                setTranslation.Add("DarkAlien", "Dark Predator");
                setTranslation.Add("PetTamer", "Beast Rancher");
                setTranslation.Add("Midas", "Golden Monarch");
                setTranslation.Add("Bazooka", "Reckless Firepower");
                setTranslation.Add("BlueKnight", "Azure Knight");
                setTranslation.Add("Rogue", "Hidden Viper");
                setTranslation.Add("Hunter", "Nimble Hunter");
                setTranslation.Add("Diamond", "Anniversary Diamond");
                setTranslation.Add("Medic", "Bone Mender");
                setTranslation.Add("Enchant", "Celestial Enchanter");
                setTranslation.Add("Musketeer", "Noble Fencer");
                setTranslation.Add("Zeus", "Thundering Deity");
                setTranslation.Add("Monk", "Eternal Monk");
                setTranslation.Add("Thief", "Shadow Disciple");
                setTranslation.Add("Platinum", "Anniversary Platinum");
                setTranslation.Add("Mage", "Defiant Spellslinger");
                setTranslation.Add("Titan", "Titan Attacker");
                setTranslation.Add("Fur", "Frost Warden");
                setTranslation.Add("Chains", "Chained Clockwork");
                setTranslation.Add("Captain", "Captain Titan");
                setTranslation.Add("Wonder", "Amazon Princess");
                setTranslation.Add("Wolf", "The Sly Wolf");
                setTranslation.Add("Green", "Corrupt Emerald Knight");
                setTranslation.Add("Dragon", "Dragon Slayer");
                setTranslation.Add("Anniversary", "Anniversary Gold");
                setTranslation.Add("Bishop", "Blessed Bishop");
                setTranslation.Add("Knight", "Solar Paragon");
                setTranslation.Add("Crow", "Midnight Raven");
                setTranslation.Add("Viking", "Viking King");
                setTranslation.Add("Cyborg", "Cybernetic Enhancements");
                setTranslation.Add("Ghost", "Phantom Presence");
                setTranslation.Add("Grill", "Grill Master");
                setTranslation.Add("Influencer", "Digital Idol");
                setTranslation.Add("GoldAngel", "Heir of Light");
                setTranslation.Add("Demon", "Heir of Shadows");
                setTranslation.Add("Baker", "Sweets and Treats");
                setTranslation.Add("Frost", "Jack Frost");
                setTranslation.Add("Reaper", "Grim Reaper");
                setTranslation.Add("Gamer", "Combo Breaker");
                setTranslation.Add("Sports", "Dedicated Fan");
                setTranslation.Add("CNY", "Lunar Festival");
                setTranslation.Add("Sled", "Sled Season");
                setTranslation.Add("Slayer", "Toxic Slayer");
                setTranslation.Add("Scarecrow", "Scarecrow Jack");
                setTranslation.Add("Surf", "Surf Strike");
                setTranslation.Add("Quinn", "Heartly Queen");
                setTranslation.Add("Rockstar", "The Rockstar");
                setTranslation.Add("LightBunny", "Defender of the Egg");
                setTranslation.Add("Poet", "The Heartbreaker");
                setTranslation.Add("Snowman", "Snow Master");
                setTranslation.Add("Tank", "Black Knight");
                //New sets
                setTranslation.Add("Corrupted", "Corrupted");
                setTranslation.Add("DarkAngel", "The Fallen Angel");
                setTranslation.Add("Stone", "Giga-Golem");
                setTranslation.Add("RockGirl", "Rock Queen");
                setTranslation.Add("Cowboy", "Sunset City Slinger");
                setTranslation.Add("MultiCast", "Forsaken Battlemage");
                setTranslation.Add("Lifeguard", "Aquatic Defender");
                setTranslation.Add("Dungeoneer", "Dungeon Explorer");
                setTranslation.Add("FairyShaman", "Forest Sylph");
                setTranslation.Add("Picnic", "Summer Sweetheart");
                setTranslation.Add("AirshipEngineer", "Chief Mechanic");
                setTranslation.Add("Witch", "Crackling Witch");
                setTranslation.Add("FairyKnight", "Shae, the Radiant Beacon");
                setTranslation.Add("RaidCaptain", "Inspiring Captain");
                setTranslation.Add("TimeWizard", "Bronzed Chronomancer");
                setTranslation.Add("Hollow", "Bone Knight");
                setTranslation.Add("Bard", "Brave Ministrel");
                setTranslation.Add("Snowboard", "Chills and Thrills");
                setTranslation.Add("FirstMate", "Inspired Lieutenant");
                setTranslation.Add("DaggerRogue", "Cutthroat Razorfist");
                setTranslation.Add("Jukk", "Tiny Titan");
                setTranslation.Add("Jade", "Annivarsary Jade");
                setTranslation.Add("Vampire", "Ancient Vampire");
                setTranslation.Add("BlueFlame", "Chaotic Alchemist");
                setTranslation.Add("Sun", "Immaculate Arbiter");
                setTranslation.Add("Chakram", "Jonalyn, the Deadly Flower");
                setTranslation.Add("Spartan", "Spartan Champion");
                setTranslation.Add("Scout", "Skywing Skirmisher");
                //GENERIC: setTranslation.Add("Travel", "");
                setTranslation.Add("FireTribe", "Woodland Warrior");
                setTranslation.Add("Twilight", "Twilight Templar");
                setTranslation.Add("Gambler", "Roll Player");
                setTranslation.Add("SoloRaid", "Cosmic Wanderer");
                setTranslation.Add("Grinch", "Festive Bandit");
                setTranslation.Add("Lolita", "Love Struck");
                setTranslation.Add("Blacksmith", "Wonderous Forgemaster");
                setTranslation.Add("SteampunkKnight", "Rygal, the Brilliant Engineer");
                setTranslation.Add("Spellmaster", "Spell Master");
                setTranslation.Add("Pyro", "Savage Pyromancer");
                setTranslation.Add("Bane", "Titan Crusher");
                setTranslation.Add("ManaMage", "Drip Witch");
                setTranslation.Add("Prestigious", "Prestigious Champion");
                setTranslation.Add("LifetimePrestige", "Ascended Guardian");
                setTranslation.Add("Gingerbread", "Gingerbread Master");
                setTranslation.Add("PlagueDoctor", "Plague Doctor");
            }
            if(!setTranslation.TryGetValue(equipment, out string value) || value.Equals(""))
            {
                value = "MISSING: " + equipment;
            }
            return value;
        }
        private static void ManageObject(JObject jObject,int index = 0)
        {
            jObject.Remove("$type");
            if(jObject.Parent != null && jObject.ContainsKey("$content")) {

                JToken content = jObject.GetValue("$content");

                if (content.Type.Equals(JTokenType.Object))
                {
                    ManageObject((JObject)content);
                }
                else if (content.Type.Equals(JTokenType.Array))
                {
                    ManageArray((JArray)content);
                }

                switch (jObject.Parent.Type)
                {
                    case JTokenType.Array:
                        JArray arrayParent = (JArray)jObject.Parent;
                        arrayParent[index] = content;
                        break;
                    case JTokenType.Property:
                        JProperty propertyParent = (JProperty)jObject.Parent;
                        propertyParent.Value = content;
                        break;
                    default:
                        Console.WriteLine(jObject.Parent.ToString());
                        break;
                }               
            } else
            {
                foreach (JProperty property in jObject.Children<JProperty>())
                {
                    ManageProperty(property);
                }
            }                    
        }
        private static void ManageArray(JArray jArray)
        {
            JArray tmpArrayLocal = (JArray)jArray.DeepClone();
            for(int i = 0; i < jArray.Count; i++)
            {
                if (jArray[i].Type.Equals(JTokenType.Object))
                {
                    ManageObject((JObject)jArray[i], i);
                }
            }
        }
        private static void ManageProperty(JProperty property)
        {
            if (property.Value.Type.Equals(JTokenType.Array))
            {
                ManageArray((JArray)property.Value);
            } else if (property.Value.Type.Equals(JTokenType.Object))
            {
                ManageObject((JObject)property.Value);
            }
        }
        private static string GetSaveFileContentString()
        {
            string decryptedFile = DecryptSaveFile("../../../ISavableGlobal.adat");
            int endPos = decryptedFile.IndexOf("playerData");
            string rawjson = decryptedFile.Substring(0, endPos - 2) + "}";
            var json = JObject.Parse(rawjson);
            SaveObject = JObject.Parse((string)json.GetValue("saveString"));
            ManageObject(SaveObject);
            return SaveObject.ToString().Replace("\\","");
        }
        private static string DecryptSaveFile(string path)
        {          
            byte[] encryptedFile = File.ReadAllBytes(path);
            byte[] vectorBytes = encryptedFile.Take(8).ToArray();
            byte[] encryptedSaveBytes = encryptedFile.Skip(8).ToArray();
            byte[] decryptKeyBytes = Enumerable.Range(0, SaveFileEncryptionKeyString.Length)
                                .Where(x => x % 2 == 0)
                                .Select(x => Convert.ToByte(SaveFileEncryptionKeyString.Substring(x, 2), 16))
                                .ToArray();
            string decryptedSave = DecryptMessageWithSingleDES(encryptedSaveBytes, decryptKeyBytes, vectorBytes);
            return decryptedSave;
        }
        private static string DecryptMessageWithSingleDES(byte[] message, byte[] key, byte[] vector)
        {
            string decryptedMessage = null;

            using (var desCryptoServiceProvider = new DESCryptoServiceProvider())
            {
                desCryptoServiceProvider.Key = key;
                desCryptoServiceProvider.IV = vector;
                desCryptoServiceProvider.Padding = PaddingMode.Zeros;

                var decryptor = desCryptoServiceProvider.CreateDecryptor(desCryptoServiceProvider.Key, desCryptoServiceProvider.IV);

                using (var msDecrypt = new MemoryStream(message))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedMessage = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return decryptedMessage;
        }
        private static string Match()
        {
            string result = "";
            JObject artifactModel = JObject.Parse(File.ReadAllText("../../../artifactModel.json"));
            JObject artifacts = JObject.Parse(File.ReadAllText("../../../artifacts.json"));
            foreach(JProperty property in artifacts.Children<JProperty>())
            {
                JObject value = (JObject)property.Value;
                string level = value.GetValue("level").ToString();
                string significand = level.Split("E+")[0];
                string exponent = level.Split("E+")[1];
                result += "";
            }
            return result;
        }
    }   
}

