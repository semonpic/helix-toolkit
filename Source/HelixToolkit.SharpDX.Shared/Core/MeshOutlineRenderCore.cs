﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    public class MeshOutlineRenderCore : PatchMeshRenderCore, IMeshOutlineParams
    {
        #region Properties
        /// <summary>
        /// Outline color
        /// </summary>
        public Color4 Color
        {
            set
            {
                SetAffectsRender(ref modelStruct.Color, value);
            }
            get
            {
                return modelStruct.Color.ToColor4();
            }
        }

        private bool outlineEnabled = false;
        /// <summary>
        /// Enable outline
        /// </summary>
        public bool OutlineEnabled
        {
            set
            {
                SetAffectsRender(ref outlineEnabled, value);
            }
            get
            {
                return outlineEnabled;
            }
        }

        private bool drawMesh = true;
        /// <summary>
        /// Draw original mesh
        /// </summary>
        public bool DrawMesh
        {
            set
            {
                SetAffectsRender(ref drawMesh, value);
            }
            get
            {
                return drawMesh;
            }
        }

        private bool drawOutlineBeforeMesh = false;
        /// <summary>
        /// Draw outline order
        /// </summary>
        public bool DrawOutlineBeforeMesh
        {
            set
            {
                SetAffectsRender(ref drawOutlineBeforeMesh, value);
            }
            get { return drawOutlineBeforeMesh; }
        }

        /// <summary>
        /// Outline fading
        /// </summary>
        public float OutlineFadingFactor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get { return modelStruct.Params.Y; }
        }

        private string outlinePassName = DefaultPassNames.MeshOutline;
        public string OutlinePassName
        {
            set
            {
                if(SetAffectsRender(ref outlinePassName, value) && IsAttached)
                {
                    outlineShaderPass = EffectTechnique[value];
                }
            }
            get
            {
                return outlinePassName;
            }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        protected IShaderPass outlineShaderPass { private set; get; }

        public MeshOutlineRenderCore()
        {
            OutlineFadingFactor = 1.5f;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            outlineShaderPass = technique[OutlinePassName];
            return base.OnAttach(technique);
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {            
            base.OnUpdatePerModelStruct(ref model, context);
            model.Params.Y = OutlineFadingFactor;
        }

        protected override void OnRender(IRenderContext context)
        {
            if (DrawOutlineBeforeMesh)
            {
                outlineShaderPass.BindShader(context.DeviceContext);
                outlineShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
            if (DrawMesh)
            {
                base.OnRender(context);
            }
            if (!DrawOutlineBeforeMesh)
            {
                outlineShaderPass.BindShader(context.DeviceContext);
                outlineShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
        }
    }

    public class MeshXRayRenderCore : MeshOutlineRenderCore
    {
        public MeshXRayRenderCore()
        {
            DrawOutlineBeforeMesh = true;
            OutlinePassName = DefaultPassNames.MeshXRay;
        }
    }
}
