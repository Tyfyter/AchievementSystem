using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AchievementSystem {
	public class AchievementSystem : Mod {
		public override void Unload() {
			AchievementLoader.Unload();
		}
	}
	public class AchievementPlayer : ModPlayer {
		public HashSet<int> achievedAchievements;
		public List<string> unloadedAchievements;
		public override void PostUpdate() {
			achievedAchievements ??= new();
			for (int i = 0; i < AchievementLoader.AchievementCount; i++) {
				if (!achievedAchievements.Contains(i) && AchievementLoader.GetAchievement(i).IsUnlocked()) {
					achievedAchievements.Add(i);
				}
			}
		}
		public override void SaveData(TagCompound tag) {
			unloadedAchievements ??= new();
			tag[nameof(achievedAchievements)] = achievedAchievements
				.Select(a => AchievementLoader.GetAchievement(a).FullName)
				.Union(unloadedAchievements)
				.ToList();
		}
		public override void LoadData(TagCompound tag) {
			achievedAchievements ??= new();
			unloadedAchievements ??= new();
			if (tag.TryGet(nameof(achievedAchievements), out List<string> achievements)) {
				foreach (string item in achievements) {
					if (ModContent.TryFind<ModAchievement>(item, out var modAchievement)) {
						achievedAchievements.Add(modAchievement.Type);
					} else {
						unloadedAchievements.Add(item);
					}
				}
			}
		}
	}
	public static class AchievementLoader {
		internal static readonly IList<ModAchievement> achievements = new List<ModAchievement>();
		private static int nextAchievement = 0;
		public static int AchievementCount => nextAchievement;
		internal static int ReserveAchievementID() {
			return nextAchievement++;
		}
		public static ModAchievement GetAchievement(int type) {
			return type >= AchievementCount ? null : achievements[type];
		}
		internal static void Unload() {
			achievements.Clear();
			nextAchievement = 0;
		}
	}
	public class ModAchievement : ModTexturedType {
		/// <summary> The file name of this type's texture file in the mod loader's file space. </summary>
		public virtual string LockedTexture => Texture + "_Locked";
		Asset<Texture2D> textureAsset;
		public Asset<Texture2D> TextureAsset => textureAsset ??= ModContent.Request<Texture2D>(Texture);
		Asset<Texture2D> lockedTextureAsset;
		public Asset<Texture2D> LockedTextureAsset => lockedTextureAsset ??= ModContent.Request<Texture2D>(LockedTexture);
		/// <summary> The achievement id of this achievement. </summary>
		public int Type { get; internal set; }

		/// <summary> The translations of this achievement's display name. </summary>
		public ModTranslation DisplayName { get; internal set; }

		/// <summary> The translations of this achievement's description. </summary>
		public ModTranslation Description { get; internal set; }
		protected sealed override void Register() {
			ModTypeLookup<ModAchievement>.Register(this);
			Type = AchievementLoader.ReserveAchievementID();
			DisplayName = LocalizationLoader.GetOrCreateTranslation(Mod, "BuffName." + Name);
			Description = LocalizationLoader.GetOrCreateTranslation(Mod, "BuffDescription." + Name);
			AchievementLoader.achievements.Add(this);
		}
		public virtual bool IsUnlocked() => false;
	}
}