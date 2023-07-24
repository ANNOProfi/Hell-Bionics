using System;
using UnityEngine;
using Verse;

namespace HellBionics
{
    [StaticConstructorOnStartup]
    public class HB_Gizmo_PlasmaStatus : Gizmo
    {
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0.5f, 0f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.3f, 0.2f, 0.15f));

        private HediffComp_InfernalUtility infernalUtility;

        public HB_Gizmo_PlasmaStatus(HediffComp_InfernalUtility infernalUtility)
        {
            this.infernalUtility = infernalUtility;
            this.Order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			Rect rect3 = rect2;
			rect3.height = rect.height / 2f;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect3, "Plasma");
			Rect rect4 = rect2;
			rect4.yMin = rect2.y + rect2.height / 2f;
			float fillPercent = this.infernalUtility.RemainingPlasma / this.infernalUtility.MaximumPlasma;
			Widgets.FillableBar(rect4, fillPercent, HB_Gizmo_PlasmaStatus.FullShieldBarTex, HB_Gizmo_PlasmaStatus.EmptyShieldBarTex, false);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect4, this.infernalUtility.RemainingPlasma.ToString("F0") + " / " + this.infernalUtility.MaximumPlasma.ToString("F0"));
			Text.Anchor = TextAnchor.UpperLeft;
			return new GizmoResult(GizmoState.Clear);
        }
    }
}