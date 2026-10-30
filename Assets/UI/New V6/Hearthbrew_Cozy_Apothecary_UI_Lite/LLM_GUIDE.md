# Cozy Apothecary UI Kit – Hearthbrew (Lite): guide for code assistants and LLMs

Kit: **126 sprite files per scale** (78 interface pieces, 24 icons and 24 disabled
icons), at 1x and 2x. When in doubt, `manifest.json` wins.

## Rules

- Paths are `1x/<path>` or `2x/<path>` with `path` from `manifest.json` (`sprites` list). 1x
  targets 1920x1080; 2x is exactly double. Use one scale per project, smooth filtering.
- Names: `{category}_{variant}[_{size}][_{state}].png`. States: `normal`, `hover`,
  `pressed`, `selected`, `focus`, `disabled`; slots add `empty`, `drag_over` and `locked`.
  Each manifest entry also has `lite_family` (the family below) and `state`.
- `hover`, `focus` and `selected` carry the amber accent; never show amber on an idle element.
- **Border order:** `nine_slice_1x` / `nine_slice_2x` are **left, top, right, bottom** (`l, t, r, b`)
  in pixels of that scale. CSS `border-image-slice` uses top, right, bottom, left instead.
  1x borders are half the 2x borders, rounded up when odd (the 1x slice never cuts into the corner art); read `nine_slice_1x`, never halve.
- `fill`: `stretch`, `tile` (repeat edges and centre), `tile_x` (repeat the horizontal centre,
  end caps fixed) or `fixed` (never resize). `resize_axes`: `x`, `y`, `xy` or `none`.
- Enforce `min_host_*` (width, height). `content_rect_*` and `fill_rect_*` are x, y, width,
  height. Bars and sliders: draw the fill across `fill_rect_*`, then clip it to the value
  (0 hides it).
- `slot_square_md_locked` has an `attachments` entry: place `slot_lock_overlay` centred at its
  fixed size; do not slice it with the slot.
- Icon buttons: `ibtn_square_md_*` is an empty body (48x48 at 1x); centre an icon
  scaled to 36x36 at 1x (72x72 at 2x) on it, for example `ui_settings`. `ibtn_close_md_*`
  already carries its x.
- Cursors are fixed 32x32 at 1x; `hotspot_1x` / `hotspot_2x` is the click pixel (x,y from the
  top-left); `hotspot_1x` is the 1x pixel that holds the 2x one, floor(hotspot_2x / 2).
- Icons are 64x64 at 1x; the disabled twin is `<id>_disabled.png` beside it.
- No baked text: Fraunces for titles and buttons, Andika for body text (`fonts/`).

## Families and borders

| Family | Group | Folder | 1x size | 9-slice 1x l,t,r,b | 9-slice 2x | Fill | Resize | States |
|---|---|---|---|---|---|---|---|---|
| `panel_window_main_9s` | Windows and panels | `panels/` | 686x453 | 64,64,64,64 | 128,128,128,128 | stretch | xy | normal |
| `panel_panel_parchment_9s` | Windows and panels | `panels/` | 320x240 | 20,20,20,20 | 40,40,40,40 | tile | xy | normal |
| `panel_panel_inset_9s` | Windows and panels | `panels/` | 240x176 | 48,48,48,48 | 96,96,96,96 | stretch | xy | normal |
| `panel_title_plate_9s` | Windows and panels | `panels/` | 128x56 | 24,0,24,0 | 48,0,48,0 | tile_x | x | normal |
| `panel_list_row_9s` | Windows and panels | `panels/` | 160x56 | 24,0,24,0 | 48,0,48,0 | tile_x | x | normal, hover, selected |
| `panel_tooltip_9s` | Windows and panels | `panels/` | 200x88 | 20,20,20,20 | 40,40,40,40 | tile | xy | normal |
| `divider_rule_oak` | Windows and panels | `dividers/` | 96x10 | 11,0,11,0 | 22,0,22,0 | tile_x | x | normal |
| `btn_primary_md` | Buttons | `button/` | 160x60 | 28,13,28,13 | 56,26,56,25 | stretch | x | normal, hover, pressed, selected, focus, disabled |
| `btn_primary_sm` | Buttons | `button/` | 112x48 | 26,0,26,0 | 52,0,52,0 | stretch | x | normal, hover, pressed, focus, disabled |
| `ibtn_close_md` | Buttons | `icon_button/` | 48x48 | - | - | fixed | none | normal, hover, pressed, focus, disabled |
| `ibtn_square_md` | Buttons | `icon_button/` | 48x48 | - | - | fixed | none | normal, hover, pressed, selected, focus, disabled |
| `toggle_checkbox_off` | Controls | `toggle/` | 32x32 | - | - | fixed | none | normal, hover, pressed, focus, disabled |
| `toggle_checkbox_on` | Controls | `toggle/` | 32x32 | - | - | fixed | none | normal, hover, pressed, focus, disabled |
| `toggle_switch_off` | Controls | `toggle/` | 64x32 | - | - | fixed | none | normal, hover, focus, disabled |
| `toggle_switch_on` | Controls | `toggle/` | 64x32 | - | - | fixed | none | normal, hover, focus, disabled |
| `slider_track_h_9s` | Controls | `slider/` | 192x16 | 12,0,12,0 | 24,0,24,0 | tile_x | x | normal |
| `slider_fill_h_sage` | Controls | `slider/` | 32x4 | 0,0,0,0 | 0,0,0,0 | tile_x | x | normal |
| `slider_handle` | Controls | `slider/` | 32x32 | - | - | fixed | none | normal, hover, pressed, focus, disabled |
| `tab_top` | Controls | `tab/` | 96x36 | 24,0,24,0 | 48,0,48,0 | tile_x | x | normal, hover, selected, focus, disabled |
| `bar_frame_md_9s` | Bars, slots and HUD | `bar/` | 240x24 | 19,9,19,9 | 38,18,38,18 | stretch | xy | normal |
| `bar_fill_health` | Bars, slots and HUD | `bar/` | 48x18 | 9,0,9,0 | 18,0,18,0 | tile_x | x | normal |
| `bar_fill_mana` | Bars, slots and HUD | `bar/` | 48x18 | 9,0,9,0 | 18,0,18,0 | tile_x | x | normal |
| `bar_fill_stamina` | Bars, slots and HUD | `bar/` | 48x18 | 9,0,9,0 | 18,0,18,0 | tile_x | x | normal |
| `slot_square_md` | Bars, slots and HUD | `slot/` | 64x64 | 18,18,18,18 | 36,36,36,36 | stretch | xy | normal, hover, selected, focus, disabled, empty, drag_over, locked |
| `slot_lock_overlay` | Bars, slots and HUD | `slot/` | 24x30 | - | - | fixed | none | locked |
| `slot_qty_badge_9s` | Bars, slots and HUD | `slot/` | 64x36 | 20,0,20,0 | 40,0,40,0 | tile_x | x | normal |
| `hud_coin_purse_plate_9s` | Bars, slots and HUD | `hud/` | 96x36 | 39,6,10,6 | 78,12,20,12 | stretch | x | normal |

## Cursors

| Id | 1x path | 1x size | Hotspot 1x | Hotspot 2x | Use when |
|---|---|---|---|---|---|
| `cursor_arrow` | `cursors/cursor_arrow.png` | 32x32 | 4,4 | 8,8 | arrow cursor; the hotspot is the ink tip |
| `cursor_hand_point` | `cursors/cursor_hand_point.png` | 32x32 | 11,3 | 22,6 | link / clickable: pointing hand, the hotspot is the fingertip |

## Icons (24, each with `_disabled`)

| Id | 1x path | Use when |
|---|---|---|
| `shop_coin_gold` | `icons/shop_economy/shop_coin_gold.png` | Currency or trade icon: coin gold. |
| `status_vital_health` | `icons/status/status_vital_health.png` | Status indicator: vital health. |
| `potion_health_small` | `icons/potions_bottles/potion_health_small.png` | Potion or bottle icon: potion health small. |
| `potion_mana_small` | `icons/potions_bottles/potion_mana_small.png` | Potion or bottle icon: potion mana small. |
| `herb_sage_leaf` | `icons/herbs_flowers/herb_sage_leaf.png` | Herb or flower icon: sage leaf. |
| `shop_key` | `icons/shop_economy/shop_key.png` | Currency or trade icon: shop key. |
| `shop_chest_small` | `icons/shop_economy/shop_chest_small.png` | Currency or trade icon: chest small. |
| `shop_reputation_star` | `icons/shop_economy/shop_reputation_star.png` | Currency or trade icon: reputation star. |
| `tool_cauldron` | `icons/tools_crafting/tool_cauldron.png` | Crafting tool icon: cauldron. |
| `tool_mortar_pestle` | `icons/tools_crafting/tool_mortar_pestle.png` | Crafting tool icon: mortar pestle. |
| `ui_inventory_bag` | `icons/ui_system/ui_inventory_bag.png` | Interface action icon: inventory bag. |
| `quest_scroll` | `icons/quests_social/quest_scroll.png` | Quest or social icon: quest scroll. |
| `ui_map` | `icons/ui_system/ui_map.png` | Interface action icon: map. |
| `ui_settings` | `icons/ui_system/ui_settings.png` | Interface action icon: settings. |
| `ui_volume_on` | `icons/ui_system/ui_volume_on.png` | Interface action icon: volume on. |
| `ui_volume_off` | `icons/ui_system/ui_volume_off.png` | Interface action icon: volume off. |
| `ui_play` | `icons/ui_system/ui_play.png` | Interface action icon: play. |
| `ui_pause` | `icons/ui_system/ui_pause.png` | Interface action icon: pause. |
| `nav_arrow_left` | `icons/navigation/nav_arrow_left.png` | Navigation or travel item icon: arrow left. |
| `nav_arrow_right` | `icons/navigation/nav_arrow_right.png` | Navigation or travel item icon: arrow right. |
| `ui_info` | `icons/ui_system/ui_info.png` | Interface action icon: info. |
| `ui_close` | `icons/ui_system/ui_close.png` | Interface action icon: close. |
| `ui_check` | `icons/ui_system/ui_check.png` | Interface action icon: check. |
| `ui_menu` | `icons/ui_system/ui_menu.png` | Interface action icon: menu. |

## Example prompt

"Using the Hearthbrew Lite kit (`1x/`, read `manifest.json`), build a 1920x1080 settings
screen. Draw `panel_window_main_9s` as a 9-slice at least 600x430 with its `nine_slice_1x`
borders and fill mode, a `panel_title_plate_9s` with the title in Fraunces, then three
`panel_list_row_9s_normal` rows: Music volume with `slider_track_h_9s`, `slider_fill_h_sage`
clipped at the handle and `slider_handle_normal`; Fullscreen with `toggle_switch_on_normal`;
Subtitles with `toggle_checkbox_off_normal`. Put `ibtn_close_md_normal` in the top-right corner
and a `btn_primary_md_normal` Save button at the bottom, label in Fraunces inside its
`content_rect_1x`. Use the hover, pressed, focus and disabled files for those states."
