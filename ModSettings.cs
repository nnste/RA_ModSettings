using Verse;
using UnityEngine;
using System.Collections.Generic;
using ModSettingRegistry;
using SharedDefs;

namespace ModSettingsUI
{
    public class ModSettings : Mod
    {
        private Vector2 scroll = Vector2.zero;
        private ParentSection currentSection = ParentSection.Core;
        private SettingCategory currentCategory = SettingCategory.Always;

        // 색상 팔레트
        readonly Color BgHeader = new Color(0.15f, 0.15f, 0.15f);
        readonly Color HeaderText = Color.white;

        readonly Color BgNormal = new Color(0.25f, 0.25f, 0.25f);
        readonly Color BgHover = new Color(0.35f, 0.35f, 0.35f);
        readonly Color BgSelected = new Color(0.75f, 0.75f, 0.75f);
        readonly Color BgDisabled = new Color(0.18f, 0.18f, 0.18f);

        readonly Color BorderNormal = new Color(0.3f, 0.3f, 0.3f);
        readonly Color BorderHover = new Color(0.5f, 0.5f, 0.5f);
        readonly Color BorderSelected = new Color(0.75f, 0.75f, 0.75f);

        readonly Color TextNormal = Color.white;
        readonly Color TextSelected = Color.white;
        readonly Color TextDisabled = new Color(0.5f, 0.5f, 0.5f);



        const float InsetPadding = 6f;

        public ModSettings(ModContentPack content) : base(content) { }

        public override string SettingsCategory()
        {
            return "래비 애드온 설정";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float leftWidth = 200f;
            float gap = 10f;

            Rect leftPanel = new Rect(inRect.x, inRect.y, leftWidth, inRect.height);
            Rect rightPanel = new Rect(inRect.x + leftWidth + gap, inRect.y, inRect.width - leftWidth - gap, inRect.height);

            DrawLeftPanel(leftPanel);
            DrawRightPanel(rightPanel);
        }

        private void DrawLeftPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);

            listing.Gap(4f);
            // 헤더
            Rect headerFull = listing.GetRect(30f);
            Rect headerInset = ApplyInset(headerFull);
            DrawPanelHeader(headerInset, "◆ DLC / 모드 분류");

            listing.Gap(4f);

            // DLC 버튼들
            DrawSectionButton(listing, "코어", ParentSection.Core, true);
            DrawSectionButton(listing, "로얄티", ParentSection.Royalty, ModsConfig.RoyaltyActive);
            DrawSectionButton(listing, "이데올로기", ParentSection.Ideology, ModsConfig.IdeologyActive);
            DrawSectionButton(listing, "바이오테크", ParentSection.Biotech, ModsConfig.BiotechActive);
            DrawSectionButton(listing, "어노말리", ParentSection.Anomaly, ModsConfig.AnomalyActive);
            DrawSectionButton(listing, "오딧세이", ParentSection.Odyssey, ModsConfig.OdysseyActive);

            listing.End();
        }

        private void DrawPanelHeader(Rect rect, string label)
        {
            Widgets.DrawBoxSolid(rect, BgHeader);

            TextAnchor oldAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;

            Rect paddedRect = new Rect(rect.x + 8f, rect.y, rect.width - 8f, rect.height);

            Color oldColor = GUI.color;
            GUI.color = HeaderText;
            Widgets.Label(paddedRect, label);
            GUI.color = oldColor;

            Text.Anchor = oldAnchor;
        }

        private void DrawSectionButton(Listing_Standard listing, string label, ParentSection section, bool isEnabled)
        {
            Rect fullRect = listing.GetRect(42f);
            Rect paddedRect = ApplyInset(fullRect);

            bool clicked = DrawFlatButton(paddedRect, label, currentSection == section, isEnabled);

            if (clicked && isEnabled)
            {
                currentSection = section;
            }

            listing.Gap(4f);
        }

        private Rect ApplyInset(Rect rect)
        {
            return new Rect(rect.x + InsetPadding, rect.y, rect.width - (InsetPadding * 2f), rect.height);
        }

        private void DrawRightPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            float tabsHeight = 42f;
            Rect tabsRect = new Rect(rect.x + 10f, rect.y + 10f, rect.width - 20f, tabsHeight);
            DrawCategoryTabs(tabsRect);

            float contentY = rect.y + tabsHeight + 20f;
            float contentHeight = rect.height - tabsHeight - 30f;
            Rect contentRect = new Rect(rect.x + 10f, contentY, rect.width - 20f, contentHeight);

            DrawContentArea(contentRect);
        }

        private void DrawCategoryTabs(Rect rect)
        {
            float buttonWidth = (rect.width - 30f) / 4f;
            float x = rect.x;

            DrawCategoryButton(new Rect(x, rect.y, buttonWidth, 36f), "상시적용", SettingCategory.Always);
            x += buttonWidth + 10f;
            DrawCategoryButton(new Rect(x, rect.y, buttonWidth, 36f), "기초", SettingCategory.Early);
            x += buttonWidth + 10f;
            DrawCategoryButton(new Rect(x, rect.y, buttonWidth, 36f), "중반", SettingCategory.Mid);
            x += buttonWidth + 10f;
            DrawCategoryButton(new Rect(x, rect.y, buttonWidth, 36f), "후반", SettingCategory.Late);
        }

        private void DrawCategoryButton(Rect rect, string label, SettingCategory category)
        {
            bool clicked = DrawFlatButton(rect, label, currentCategory == category, true);
            if (clicked)
            {
                currentCategory = category;
            }
        }

        private void DrawContentArea(Rect rect)
        {
            Rect viewRect = new Rect(0, 0, rect.width - 16, 2000f);
            Widgets.BeginScrollView(rect, ref scroll, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            bool found = false;

            foreach (var item in Registry.AllItems)
            {
                if (item.Section != currentSection) continue;
                if (item.Category != currentCategory) continue;
                found = true;
                DrawDefinition(listing, item);
                listing.GapLine();
            }

            foreach (var group in Registry.AllGroups)
            {
                if (group.Section != currentSection) continue;
                if (group.Category != currentCategory) continue;
                found = true;
                DrawGroup(listing, group);
                listing.GapLine();
            }

            if (!found)
            {
                listing.Label(" 이 카테고리에 설정 항목이 없습니다.");
            }

            listing.End();
            Widgets.EndScrollView();

            Registry.ExposeAll();
        }

        private bool DrawFlatButton(Rect rect, string label, bool isSelected, bool isActive = true)
        {
            bool isHover = Mouse.IsOver(rect);
            Color bgColor;
            Color borderColor;
            Color textColor = TextNormal;

            if (!isActive)
            {
                bgColor = BgDisabled;
                borderColor = BorderNormal;
                textColor = TextDisabled;
            }
            else if (isSelected)
            {
                bgColor = BgSelected;
                borderColor = BorderSelected;
                textColor = TextSelected;
            }
            else if (isHover)
            {
                bgColor = BgHover;
                borderColor = BorderHover;
            }
            else
            {
                bgColor = BgNormal;
                borderColor = BorderNormal;
            }

            Widgets.DrawBoxSolid(rect, bgColor);
            DrawBorder(rect, borderColor);

            TextAnchor oldAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Color oldColor = GUI.color;
            GUI.color = textColor;
            Widgets.Label(rect, label);
            GUI.color = oldColor;
            Text.Anchor = oldAnchor;

            return isActive && Widgets.ButtonInvisible(rect);
        }

        private void DrawBorder(Rect rect, Color color)
        {
            Widgets.DrawBoxSolid(new Rect(rect.x, rect.y, rect.width, 1f), color);
            Widgets.DrawBoxSolid(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), color);
            Widgets.DrawBoxSolid(new Rect(rect.x, rect.y, 1f, rect.height), color);
            Widgets.DrawBoxSolid(new Rect(rect.xMax - 1f, rect.y, 1f, rect.height), color);
        }

        private void DrawDefinition(Listing_Standard listing, SettingDefinition def)
        {
            if (def.Type == SettingType.Toggle)
            {
                bool val = def.Getter();
                listing.CheckboxLabeled(def.Label, ref val, def.Description);
                def.Setter(val);
                listing.Gap();
            }
            else if (def.Type == SettingType.Slider)
            {
                float val = def.SliderGetter();
                listing.Label(def.Label + $" : {val:F2}");
                val = listing.Slider(val, def.SliderMin, def.SliderMax);
                def.SliderSetter(val);
                listing.Label(def.Description);
                listing.Gap();
            }
            else if (def.Type == SettingType.Dropdown)
            {
                int val = def.DropdownGetter();
                if (Widgets.ButtonText(listing.GetRect(30f), def.DropdownOptions[val]))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    for (int i = 0; i < def.DropdownOptions.Count; i++)
                    {
                        int index = i;
                        options.Add(new FloatMenuOption(def.DropdownOptions[i], () =>
                        {
                            val = index;
                            def.DropdownSetter(val);
                        }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                }
                listing.Label(def.Description);
                listing.Gap();
            }
            else if (def.Type == SettingType.Description)
            {
                listing.Label(def.Label);
                listing.Label(def.Description);
                listing.Gap();
            }
        }

        private void DrawGroup(Listing_Standard listing, SettingGroup group)
        {
            listing.Label($"<b>{group.GroupLabel}</b>");
            if (!string.IsNullOrEmpty(group.GroupDescription))
                listing.Label(group.GroupDescription);
            listing.Gap();

            bool masterEnabled = true;

            for (int i = 0; i < group.Items.Count; i++)
            {
                var def = group.Items[i];

                if (i == 0 && def.Type == SettingType.Toggle)            // ① 첫 항목 = 마스터 토글
                {
                    DrawDefinition(listing, def);                        //   정상 출력 & 값 갱신
                    masterEnabled = def.Getter();                       //   갱신된 ON/OFF 확인
                    listing.Indent(1);                                   //   서브 항목 시각적 들여쓰기
                    continue;
                }

                bool prevGui = GUI.enabled;                              // ② 마스터 OFF → 회색 + 입력 차단
                if (!masterEnabled) GUI.enabled = false;

                DrawDefinition(listing, def);

                GUI.enabled = prevGui;
            }
            listing.Indent(0);                                           // 들여쓰기 원상복구
        }

    }
}
