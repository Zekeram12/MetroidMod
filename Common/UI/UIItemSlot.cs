﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace MetroidMod.Common.UI
{
	public abstract class UIItemSlot : UIElement
	{
		public Texture2D backgroundTexture;
		internal float scale;
		internal Item ItemRead;
		public Item ItemWrite;
		internal protected event Func<bool> CanClick;

		public UIItemSlot(float scale = 1f)
		{
			this.scale = scale;
			ItemRead = new Item();
			backgroundTexture = TextureAssets.InventoryBack4.Value;
			Width.Set(backgroundTexture.Width * scale, 0f);
			Height.Set(backgroundTexture.Height * scale, 0f);
		}

		public override void LeftMouseDown(UIMouseEvent evt)
		{
			Player player = Main.LocalPlayer;

			if (player.itemAnimation == 0 && player.itemTime == 0)
			{

				if (CanClick?.Invoke() ?? true)
				{
					MetroidMod.Instance.Logger.Info("Hey if this is showing up there's probably a problem");
					SlotMagic(true);
					base.LeftMouseDown(evt);
				}
			}
		}

		public void SlotMagic(bool itsGiving)
		{
			MetroidMod.Instance.Logger.Info("MAGIC TIME");
			if (itsGiving)
			{
				MetroidMod.Instance.Logger.Info("ermmm.......... it's GIVING");
				if (ItemRead == Main.mouseItem)
				{
					MetroidMod.Instance.Logger.Info("Looks like we gots a stackem on our hands fellas");
					Item ItemWrite = Main.mouseItem;
					Main.mouseItem.TurnToAir();
					DarkMagic(ItemWrite, true);
					SoundEngine.PlaySound(SoundID.Grab);
				}
				else
				{
					MetroidMod.Instance.Logger.Info("Ain't stacking.");
					Item ItemWrite = Main.mouseItem;
					Main.mouseItem = ItemRead;
					MetroidMod.Instance.Logger.Info("You should now be holding something called " + Main.mouseItem + ", and it should be the same as " + ItemRead);
					DarkMagic(ItemWrite, false);
					SoundEngine.PlaySound(SoundID.Grab);
				}

			}
			else
			{
				MetroidMod.Instance.Logger.Info("it is absolutely not giving (HOTLOADED PROPERLY!!!");
				Main.mouseItem = ItemRead.Clone();
				MetroidMod.Instance.Logger.Info("Hand: " + Main.mouseItem + "\nPicking up: " + ItemRead);
				DarkMagic(ItemWrite = null, false);
				Recipe.FindRecipes();
				SoundEngine.PlaySound(SoundID.Grab);
			}
		}

		/// <summary>
		/// Use this to write the mouse item to the array.
		/// <br/>Do <b>NOT</b> write over ItemRead during this. It'll be fine on its own.
		/// </summary>
		public abstract void DarkMagic(Item ItemWrite, bool StackAttack);

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 position = GetInnerDimensions().Position();
			spriteBatch.Draw(backgroundTexture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

			if (ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			if (ItemRead != null && !ItemRead.IsAir)
			{
				Texture2D itemTexture = TextureAssets.Item[ItemRead.type].Value;
				Rectangle textureFrame = Main.itemAnimations[ItemRead.type]?.GetFrame(itemTexture) ?? itemTexture.Bounds;

				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, ItemRead, false);
				int height = textureFrame.Height;
				int width = textureFrame.Width;
				float drawScale = 1f;
				float availableWidth = 32; // defaultBackgroundTexture.Width * scale;
				if (width > availableWidth || height > availableWidth)
				{
					drawScale = availableWidth / (width > height ? width : height);
				}
				drawScale *= scale;
				Vector2 itemPosition = position + backgroundTexture.Size() * scale / 2f - textureFrame.Size() * drawScale / 2f;
				Vector2 itemOrigin = textureFrame.Size() * (pulseScale / 2f - 0.5f);
				if (ItemLoader.PreDrawInInventory(ItemRead, spriteBatch, itemPosition, textureFrame, ItemRead.GetAlpha(newColor),
					ItemRead.GetColor(Color.White), itemOrigin, drawScale * pulseScale))
				{
					spriteBatch.Draw(itemTexture, itemPosition, textureFrame, ItemRead.GetAlpha(newColor), 0f, itemOrigin, drawScale * pulseScale, SpriteEffects.None, 0f);
					if (ItemRead.color != Color.Transparent)
					{
						spriteBatch.Draw(itemTexture, itemPosition, textureFrame, ItemRead.GetColor(Color.White), 0f, itemOrigin, drawScale * pulseScale, SpriteEffects.None, 0f);
					}
				}
				ItemLoader.PostDrawInInventory(ItemRead, spriteBatch, itemPosition, textureFrame, ItemRead.GetAlpha(newColor),
					ItemRead.GetColor(Color.White), itemOrigin, drawScale * pulseScale);
				if (ItemID.Sets.TrapSigned[ItemRead.type])
				{
					spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
				}

				if (ItemRead.stack > 1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, ItemRead.stack.ToString(), position + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
				}

				if (IsMouseHovering)
				{
					Main.HoverItem = ItemRead.Clone();
					Main.hoverItemName = Main.HoverItem.Name;
				}
			}
		}
	}
}
