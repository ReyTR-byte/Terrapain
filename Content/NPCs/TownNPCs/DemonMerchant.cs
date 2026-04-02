using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.System;
using Terrapain.Content.Items.Consumables.Potions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Terrapain.Content.NPCs.TownNPCs
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    [AutoloadHead]
    public class DemonMerchant : ModNPC
    {
        public const string ShopName = "Shop";
        public int NumberOfTimesTalkedTo = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public static LocalizedText UpgradedText { get; private set; }

        // Sets a unique message when the NPC dies.
        // See also NPCID.Sets.IsTownChild if you just want the message used by Angler and Princess.
        // See ModifyDeathMessage() way below for more details
        public override LocalizedText DeathMessage => this.GetLocalization("DeathMessage");

        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            //ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
            NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
            NPCID.Sets.AttackType[Type] = 0; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
            NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
            NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
            NPCID.Sets.ShimmerTownTransform[Type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.

            // Connects this NPC with a custom emote.
            // This makes it when the NPC is in the world, other NPCs will "talk about him".
            // By setting this you don't have to override the PickEmote method for the emote to appear.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
                              // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
                              // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Like) // Example Person prefers the forest.
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike) // Dislikes living near the merchant.
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate) // Hates living near the demolitionist.
            ; // < Mind the semicolon!

            // This creates a "profile" for ExamplePerson, which allows for different textures during a party and/or while the NPC is shimmered.
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture/* + "_Party"*/)
                //new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
            );

            ContentSamples.NpcBestiaryRarityStars[Type] = 3; // We can override the default bestiary star count calculation by setting this.

            UpgradedText = this.GetLocalization("Upgraded");
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; 
            NPC.friendly = true; 
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 15;
            NPC.defense = 20;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Clothier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange([
                // Sets the preferred biomes of this town NPC listed in the bestiary.
                // With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// Sets your NPC's flavor text in the bestiary. (use localization keys)
				new FlavorTextBestiaryInfoElement("Mods.Terrapain.Bestiary.ExamplePerson_1"),

				// You can add multiple elements if you really wanted to
				new FlavorTextBestiaryInfoElement("Mods.Terrapain.Bestiary.ExamplePerson_2")
            ]);
        }

        // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
        // Returning false will allow you to manually draw your NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This code slowly rotates the NPC in the bestiary
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                drawModifiers.Rotation += 0.001f;

                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch);
            }

            // Create gore when the NPC is killed.
            //if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            //{
            //    // Retrieve the gore types. This NPC has shimmer and party variants for head, arm, and leg gore. (12 total gores)
            //    string variant = "";
            //    if (NPC.IsShimmerVariant)
            //        variant += "_Shimmer";
            //    if (NPC.altTexture == 1)
            //        variant += "_Party";
            //    int hatGore = NPC.GetPartyHatGore();
            //    int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
            //    int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
            //    int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

            //    // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            //    if (hatGore > 0)
            //    {
            //        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
            //    }
            //    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            //    Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
            //    Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
            //    Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            //    Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            //}
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_SpawnNPC)
            {
                TownNPCRespawnSystem.UnlockDemonMerchantRespawn = true;
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.downedBoss2)
            {
                return true;
            }

            return false;
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Демон торговец",
            };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            //int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            //if (partyGirl >= 0 && Main.rand.NextBool(4))
            //{
            //    chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            //}
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.StandardDialogue4"));
            chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.CommonDialogue"), 5.0);
            chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.RareDialogue"), 0.1);

            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                // This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
                chat.Add(Language.GetTextValue("Mods.Terrapain.Dialogue.DemonMerchant.TalkALot"));
            }

            string chosenChat = chat; // chat is implicitly cast to a string. This is where the random choice is made.

            return chosenChat;
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = ShopName; // Name of the shop tab we want to open.
            }
        }
        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = Language.GetTextValue("LegacyInterface.28"); 
        }


        // Not completely finished, but below is what the NPC will sell
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
                .Add(ModContent.ItemType<ChaosPotion>())
                .Add(ModContent.ItemType<BrokenHeartPotion>());
                //.Add(ModContent.ItemType<ShortSwordPotion>())
                //.Add(ModContent.ItemType<ManaStealPotin>())
                //.Add(ModContent.ItemType<SniperPotin>())
                //.Add(ModContent.ItemType<MinionLovingPotion>());
            npcShop.Register(); // Name of this shop tab
        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered. Return toKingStatue for only King Statues. Return !toKingStatue for only Queen Statues. Return true for both.
        public override bool CanGoToStatue(bool toKingStatue) => true;

        //public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        //{
        //    damage = 20;
        //    knockback = 4f;
        //}

        //public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        //{
        //    cooldown = 30;
        //    randExtraCooldown = 30;
        //}

        //public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        //{
        //    projType = ModContent.ProjectileType<SparklingBall>();
        //    attackDelay = 1;
        //}

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
            // SparklingBall is not affected by gravity, so gravityCorrection is left alone.
        }

        public override void LoadData(TagCompound tag)
        {
            NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
        }
    }
}
