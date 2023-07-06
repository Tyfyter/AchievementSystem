using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AchievementSystem {
	public class AchievementSystem : Mod {
		public override void Unload() {
			AchievementLoader.Unload();
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
	}
}