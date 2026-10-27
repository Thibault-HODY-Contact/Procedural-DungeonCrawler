# Cozy Apothecary UI Kit – Hearthbrew (Lite) v1.0 (free)

A complete small kit taken from Cozy Apothecary UI Kit – Hearthbrew, a cozy apothecary game UI kit in a painterly style:
warm oak frames, parchment interiors, copper corners and hinges, with an amber accent for the
active state. There is enough here to finish a few real screens: a settings menu, a small
inventory and a HUD. The Lite is free and uses the **same licence terms as the full pack**, so
commercial use is allowed.

**AI-assisted:** base images were generated with AI, then keyed, cleaned and assembled into modular sprites by code. Artwork and functionality went through automated checks and independent AI-agent review. The build tools and documentation are also AI-assisted.

## What is inside

**102 items**: 78 interface pieces and 24 icons. Every icon also ships a
disabled (greyed) twin, so each scale folder holds **126 sprite files**
(102 + 24 disabled icons), in 29 interface families.

- **Windows and panels:** `panel_window_main_9s`, `panel_panel_parchment_9s`, `panel_panel_inset_9s`, `panel_title_plate_9s`, `panel_list_row_9s`, `panel_tooltip_9s`, `divider_rule_oak`.
- **Buttons:** one text-button style in every state at two sizes, a close button and a square
  icon button for the settings icon (or any other): `btn_primary_md`, `btn_primary_sm`, `ibtn_close_md`, `ibtn_square_md`.
- **Controls:** checkbox, switch, slider and top tabs: `toggle_checkbox_off`, `toggle_checkbox_on`, `toggle_switch_off`, `toggle_switch_on`, `slider_track_h_9s`, `slider_fill_h_sage`, `slider_handle`, `tab_top`.
- **Bars, slots and HUD:** `bar_frame_md_9s`, `bar_fill_health`, `bar_fill_mana`, `bar_fill_stamina`, `slot_square_md`, `slot_lock_overlay`, `slot_qty_badge_9s`, `hud_coin_purse_plate_9s`.
- **Cursors:** an arrow and a pointing hand, with their hotspots in the manifest: `cursor_arrow`, `cursor_hand_point`.
- **24 icons** (in `icons/herbs_flowers/`, `icons/navigation/`, `icons/potions_bottles/`, `icons/quests_social/`, `icons/shop_economy/`, `icons/status/`, `icons/tools_crafting/`, `icons/ui_system/`), each with `_disabled`: `shop_coin_gold`, `status_vital_health`, `potion_health_small`, `potion_mana_small`, `herb_sage_leaf`, `shop_key`, `shop_chest_small`, `shop_reputation_star`, `tool_cauldron`, `tool_mortar_pestle`, `ui_inventory_bag`, `quest_scroll`, `ui_map`, `ui_settings`, `ui_volume_on`, `ui_volume_off`, `ui_play`, `ui_pause`, `nav_arrow_left`, `nav_arrow_right`, `ui_info`, `ui_close`, `ui_check`, `ui_menu`.

```
Hearthbrew_Cozy_Apothecary_UI_Lite/
  1x/            sprites for a 1920x1080 reference resolution
  2x/            the same sprites at double size, for 3840x2160 (4K) or sharp scaling
  manifest.json  every sprite: path, sizes, state, 9-slice borders, fill, resize rules, use_when
  previews/lite_contents.png   every sprite in this kit on one sheet, labelled
  fonts/         the two free font families (Fraunces, Andika) with their licences
  README.md  LLM_GUIDE.md  CHANGELOG.md  LICENSE.txt
```

## Using the sprites

1. Pick `1x/` (laid out for 1920x1080) or `2x/` (3840x2160); use one folder per project and
   smooth (bilinear) filtering. These are high-resolution sprites, not pixel art.
2. Look the sprite up in `manifest.json` (`sprites` list) by `id` or `path`.
3. If `sliced` is true, read `nine_slice_1x` (or `nine_slice_2x`): borders in the order left,
   top, right, bottom (`l, t, r, b`) in pixels of that scale. 1x borders are half the 2x borders, rounded up when odd (the 1x slice never cuts into the corner art), so
   read the 1x borders instead of halving the 2x ones. `fill` says how the edges and centre
   resize (`stretch`, `tile`, `tile_x`); `fixed` sprites are never stretched.
4. Cursors: set `hotspot_1x` (or `hotspot_2x`) as the click point, x,y from the top-left.
5. Respect `min_host_*` and `resize_axes`; put labels and icons inside `content_rect_*`
   (`x, y, width, height`). Bar and slider fills go inside `fill_rect_*`: draw the fill at full
   length, then clip it to the value.
6. There is no baked text: draw labels with the included fonts.

`LLM_GUIDE.md` has a table of every family with its borders, for code assistants.

## What the full pack adds

The full Cozy Apothecary UI Kit – Hearthbrew costs **$4.99** on itch.io. It has **1,146 items**
(488 interface pieces and 658 icons, 1,804 files per scale) at 1x and 2x:

- 658 icons in 13 sets (cooking food, creatures, herbs flowers, home decor, ingredients, navigation, potions bottles, quests social, shop economy, skills, status, tools crafting, ui system), each with a disabled version; the Lite has 24
- more text-button styles: secondary, hero, buy, danger, outline and pill, plus a wide size
- round and large square icon buttons, and minus and plus steppers
- dark and ornate windows, a dialogue box with a name plate, a popup, toasts, a journal page, a recipe-book spread, notes, shelf planks, header bands and choice rows
- text, number, search and dropdown fields, radio buttons, scroll bars, a vertical slider and bookmark tabs
- small, large, vertical and ornate bar frames, XP, brewing and warning fills, potion-bottle bars, a cauldron meter, radial meters, pips and relationship hearts
- hotbar and shelf slots, rarity frames, ingredient and result slots
- recipe, item and request cards, price tags, shop fixtures, a calendar, a clock with a season wheel, skill-tree nodes, map markers, key prompts, a quest tracker, speech bubbles and a tool wheel
- banners and ribbons, copper corners, hinges, herb sprigs and other ornaments, portrait and picture frames, more dividers
- 12 cursors with hotspots (the Lite has 2), an 8-frame loading animation, 10 effect frames and 6 backgrounds
- 13 example screens with layout files, catalog.html, catalog.json and contact sheets per category
- the full LLM_GUIDE.md that lists every sprite

Everything in this Lite is in the full pack, byte for byte, so a project can move up without
changing a file.

## Fonts

- **Fraunces** by The Fraunces Project Authors (Undercase Type), SIL Open Font License 1.1 (`fonts/LICENSE-Fraunces-OFL.txt`)
- **Andika** by SIL International, SIL Open Font License 1.1 (`fonts/LICENSE-Andika-OFL.txt`)

## Licence in short

- Use the sprites in any number of personal and commercial projects, with no royalties. Credit
  is appreciated, not required.
- Modify, recolour, crop, animate and combine them.
- Do not resell or redistribute the sprites themselves, even modified, and do not upload them to
  public repositories or asset stores.
- Do not use them to train or fine-tune generative AI models.

The full text is in `LICENSE.txt` (the full pack's licence; only its opening line differs). Questions and requests:
https://brunkagames.itch.io (leave a comment on the pack page).
