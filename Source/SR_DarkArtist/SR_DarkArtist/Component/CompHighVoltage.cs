using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SR.DA.Component
{
    public class CompHighVoltage : ThingComp
    {
        private CompProperties_HighVoltage Props
        {
            get
            {
                return (CompProperties_HighVoltage)this.props;
            }
        }
        private Texture2D cachedCommandTex;
        private Texture2D CommandTex
        {
            get
            {
                if (this.cachedCommandTex == null)
                {
                    this.cachedCommandTex = ContentFinder<Texture2D>.Get(this.Props.commandTexture, true);
                }
                return this.cachedCommandTex;
            }
        }
        private bool switchIsOn = false;//是否开启高压电
        private Graphic offGraphic;
        private const string OffGraphicSuffix = "_Off";
        public const string HighVoltageOnSignal = "HighVoltageOn";//与compEffectElectrocutionChair交互的信号
        public const string HighVoltageOffSignal = "HighVoltageOff";
        public bool SwitchIsOn
        {
            get
            {
                return this.switchIsOn;
            }
            set
            {
                switchIsOn = value;
                if (switchIsOn)
                {
                    this.parent.BroadcastCompSignal(HighVoltageOnSignal);
                }
                else
                {
                    this.parent.BroadcastCompSignal(HighVoltageOffSignal);
                }
                if (this.parent.Spawned)
                {
                    this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
                }
            }
        }
        public Graphic CurrentGraphic
        {
            get
            {
                if (this.SwitchIsOn)
                {
                    return this.parent.DefaultGraphic;
                }
                if (this.offGraphic == null)
                {
                    this.offGraphic = GraphicDatabase.Get(this.parent.def.graphicData.graphicClass, this.parent.def.graphicData.texPath + OffGraphicSuffix, this.parent.def.graphicData.shaderType.Shader, this.parent.def.graphicData.drawSize, this.parent.DrawColor, this.parent.DrawColorTwo);
                }
                return this.offGraphic;
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.switchIsOn, "switchOn", true, false);
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (this.parent.Faction == Faction.OfPlayer)
            {
                yield return new Command_Toggle
                {
                    icon = this.CommandTex,
                    defaultLabel = this.Props.commandLabelKey.Translate(),
                    defaultDesc = this.Props.commandDescKey.Translate(),
                    isActive = (() => switchIsOn),
                    toggleAction = delegate ()
                    {
                        SwitchIsOn = !SwitchIsOn;
                    }
                };
            }
            yield break;
        }
    }
}
