using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
internal class EffectMaterialGUI_Combine_AlphaBlend : ShaderGUI
{
    //深度检测
    public enum ZTestMode
    {
        处于真实位置,
        处于最上层
    }

    //深度写入
    public enum ZWriteMode
    {
        绘制全部面,
        只绘制最上层
    }

    //剔除
    public enum CullingMode
    {
        双面渲染,
        渲染背面,
        渲染前面
    }

    //Alpha
    public enum AlphaChannel
    {
        R通道,
        G通道,
        B通道,
        A通道,
        不使用
    }

    MaterialEditor m_materialEditor;

    #region My ShaderProperties

    //Rendering Mode
    MaterialProperty m_CullingMode = null;
    MaterialProperty m_ZTestMode = null;
    MaterialProperty m_ZWriteMode = null;

    //Base Additive Mode
    MaterialProperty m_TintColor = null;
    MaterialProperty m_MainTex = null;
    MaterialProperty m_Glow = null;
    MaterialProperty m_UIClip = null;
    MaterialProperty m_UseAlpha = null;
    MaterialProperty m_AlphaChannel = null;
    MaterialProperty m_UseCustomData1xy = null;

    //Frenesl Mode
    MaterialProperty m_UseFresnel = null;

    MaterialProperty m_FresnekStr = null;

    //UVTransform Mode
    MaterialProperty m_UseUVTrans = null;
    MaterialProperty m_USpeed = null;

    MaterialProperty m_VSpeed = null;

    //Distort Mode
    MaterialProperty m_UseDistort = null;
    MaterialProperty m_DistortTex = null;
    MaterialProperty m_DistortStr = null;
    MaterialProperty m_DistortUSpeed = null;

    MaterialProperty m_DistortVSpeed = null;

    //Dissolve Mode
    MaterialProperty m_UseDissolve = null;
    MaterialProperty m_DissolveRangeUseCustomData = null;
    MaterialProperty m_DissolveTex = null;
    MaterialProperty m_DissolveStr = null;
    MaterialProperty m_DissolveRange = null;

    MaterialProperty m_DissolveRimlit = null;

    //Distort Mode
    MaterialProperty m_UseLux = null;
    MaterialProperty m_Lux = null;
    MaterialProperty m_UseMask = null;
    MaterialProperty m_UseCustomDataMaskUV = null;
    MaterialProperty m_MaskTex = null;
    MaterialProperty m_MaskUSpeed = null;
    MaterialProperty m_MaskVSpeed = null;

    #endregion

    public bool showDetails = true;
    public bool toolSwitch = true;
    public bool RendererSwitch;

    //绘制函数
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        FindProperties(props);
        m_materialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        //自定义绘制方法
        ShaderPropertiesGUI(material);
    }

    //标注，标题等待
    private static class Styles
    {
        public static GUIContent cullingMode = EditorGUIUtility.TrTextContent("Culling Switch", "剔除");
        public static GUIContent zTestMode = EditorGUIUtility.TrTextContent("ZTest Switch", "深度检测");
        public static GUIContent zWriteMode = EditorGUIUtility.TrTextContent("ZWrite Switch", "深度写入");

        public static GUIContent useCustomData1xy =
            EditorGUIUtility.TrTextContent("Use CustomData1 xy", "是否使用CustomData1.xy控制UV偏移");

        //备注信息
        public static GUIContent tintColorLabel = EditorGUIUtility.TrTextContent("Tint Color", "主光源的颜色");
        public static GUIContent mainTextureLabel = EditorGUIUtility.TrTextContent("Main Tex", "主纹理");
        public static GUIContent glowLabel = EditorGUIUtility.TrTextContent("glow", "辉光");
        public static GUIContent uiClipLabel = EditorGUIUtility.TrTextContent("UIClip", "UI剔除");
        public static GUIContent alphaChannelLabel = EditorGUIUtility.TrTextContent("Alpha", "主纹理透明通道");

        //菲涅尔效果
        public static GUIContent useFresnelLabel = EditorGUIUtility.TrTextContent("Use Fresnel", "是否启用菲涅尔效果");
        public static GUIContent fresnelStrlLabel = EditorGUIUtility.TrTextContent("Fresnel Strength", "菲涅尔强度");

        //UV运动
        public static GUIContent useUVlLabel = EditorGUIUtility.TrTextContent("Use UV Transform", "是否启用UV运动");
        public static GUIContent uvTransUlLabel = EditorGUIUtility.TrTextContent("U Speed", "UV运动U方向速度");
        public static GUIContent uvTransVlLabel = EditorGUIUtility.TrTextContent("V Speed", "UV运动V方向速度");

        //扰动
        public static GUIContent useDistortlLabel = EditorGUIUtility.TrTextContent("Use Distort", "是否启用扰动效果");
        public static GUIContent distortMapLabel = EditorGUIUtility.TrTextContent("Distort Tex", "扰动贴图");
        public static GUIContent distortStrLabel = EditorGUIUtility.TrTextContent("Distort Strength", "扰动强度");
        public static GUIContent uvDistortUlLabel = EditorGUIUtility.TrTextContent("Distort U Speed", "扰动U方向速度");
        public static GUIContent uvDistortVlLabel = EditorGUIUtility.TrTextContent("Distort V Speed", "扰动V方向速度");

        //溶解
        public static GUIContent useDissolvelLabel = EditorGUIUtility.TrTextContent("Use Dissolve", "是否启用溶解效果");

        public static GUIContent useDissolveCustomData1Label =
            EditorGUIUtility.TrTextContent("Dissolve Range Use CustomData[CustomData.z]", "是否启用range的CustomData1.z");

        public static GUIContent dissolveMapLabel = EditorGUIUtility.TrTextContent("Dissolve Tex", "溶解贴图");
        public static GUIContent dissolveStrLabel = EditorGUIUtility.TrTextContent("Dissolve Strength", "溶解边缘软硬强度");
        public static GUIContent dissolveRangeLabel = EditorGUIUtility.TrTextContent("Dissolve Range", "一次溶解的周期");
        public static GUIContent dissolveRimLabel = EditorGUIUtility.TrTextContent("Dissolve Rimlit", "溶解边缘光");

        //遮罩
        public static GUIContent useCustomDataMaskUV =
            EditorGUIUtility.TrTextContent("Use Custom DataMaskUV", " 默认CustomData会同时控制主贴图和Mask贴图的UV运动关闭后mask将不再被控制移动");
        public static GUIContent useMaskGlowLabel = EditorGUIUtility.TrTextContent("Use MaskGlow", "是否启用流光");
        public static GUIContent MaskGlowLabel = EditorGUIUtility.TrTextContent("Mask Glow", "流光强度");
        public static GUIContent useMaskLabel = EditorGUIUtility.TrTextContent("Use Distort", "是否启用扰动效果");
        public static GUIContent maskMapLabel = EditorGUIUtility.TrTextContent("Mask Tex", "遮罩贴图");
        public static GUIContent uvMaskUlLabel = EditorGUIUtility.TrTextContent("Mask U Speed", "遮罩U方向速度");
        public static GUIContent uvMaskVlLabel = EditorGUIUtility.TrTextContent("Mask V Speed", "遮罩V方向速度");

        //标题
        public static string BaseMode = "·Base Mode";
        public static string SwitchMode = "·Custom Effects----功能模块（使用才勾选）";
        public static string RimlitMode = "·Fresnel Mode------菲涅尔效果";
        public static string UVTransMode = "·UV Transform Mode-UV运动";
        public static string DistortMode = "·Distort Mode------扰动";
        public static string DissolveMode = "·Dissolve Mode-----溶解";
        public static string MaskMode = "·Mask Mode---------遮罩";
        public static string RenderingMode = "·Rendering Mode----渲染模块";
        public static string CullingMode = "Cull:  表面剔除----面片正反面渲染";
        public static string ZTestMode = "ZTest: 深度检测----效果是否位于最上层";
        public static string ZWriteMode = "ZWrite:深度写入----物体是否渲染全部可视面";
        public static string AChannelMode = "Alpha通道:";

        public static readonly string[] CullNames = System.Enum.GetNames(typeof(CullingMode));
        public static readonly string[] ZWriteNames = System.Enum.GetNames(typeof(ZWriteMode));
        public static readonly string[] ZTestNames = System.Enum.GetNames(typeof(ZTestMode));
        public static readonly string[] AlphaNames = System.Enum.GetNames(typeof(AlphaChannel));
    }

    //连接shader里面的属性
    public void FindProperties(MaterialProperty[] props)
    {
        m_CullingMode = FindProperty("_Cull", props);
        m_ZTestMode = FindProperty("_ZTest", props);
        m_ZWriteMode = FindProperty("_ZWrite", props);

        m_TintColor = FindProperty("_TintColor", props);
        m_MainTex = FindProperty("_MainTex", props);
        m_Glow = FindProperty("_glow", props);
        m_UIClip = FindProperty("_UIClip", props);
        m_UseCustomData1xy = FindProperty("_UseCustomData1xy", props);

        m_UseAlpha = FindProperty("_UseChannel", props);
        m_AlphaChannel = FindProperty("_UseAlpha", props);

        m_UseFresnel = FindProperty("_UseFresnel", props);
        m_FresnekStr = FindProperty("_FresnelStr", props);

        m_UseUVTrans = FindProperty("_UseUVTran", props);
        m_USpeed = FindProperty("_U", props);
        m_VSpeed = FindProperty("_V", props);

        m_UseDistort = FindProperty("_UseDistort", props);
        m_DistortTex = FindProperty("_DistortTex", props);
        m_DistortStr = FindProperty("_DistortStr", props);
        m_DistortUSpeed = FindProperty("_U_twi", props);
        m_DistortVSpeed = FindProperty("_V_twi", props);

        m_UseDissolve = FindProperty("_UseDissolve", props);
        m_DissolveRangeUseCustomData = FindProperty("_DissolveRangeUseCustomData", props);
        m_DissolveTex = FindProperty("_DissolveTex", props);
        m_DissolveStr = FindProperty("_DissolveStr", props);
        m_DissolveRange = FindProperty("_DissolveRange", props);
        m_DissolveRimlit = FindProperty("_DissolveRimStr", props);

        m_UseLux = FindProperty("_isLux", props);
        m_Lux = FindProperty("_Lux", props);
        m_UseMask = FindProperty("_UseMask", props);
        m_UseCustomDataMaskUV = FindProperty("_UseCustomDataMaskUV", props);
        m_MaskTex = FindProperty("_MaskTex", props);
        m_MaskUSpeed = FindProperty("_U_Mask", props);
        m_MaskVSpeed = FindProperty("_V_Mask", props);
    }

    //替换shader后的处理
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);
        material.SetFloat("_ZWrite", 0.0f);
        material.SetFloat("_ZTest", 4.0f);
    }

    //自定义绘制方法
    public void ShaderPropertiesGUI(Material material)
    {
        // Use default labelWidth使用默认的Label宽度
        EditorGUIUtility.labelWidth = 0f;

        EditorGUI.BeginChangeCheck();


        //基础模块
        showDetails = GUILayout.Toggle(showDetails, Styles.BaseMode, EditorStyles.boldLabel);
        //GUILayout.Label(Styles.BaseMode, EditorStyles.boldLabel);
        // m_materialEditor.TexturePropertySingleLine(Styles.mainTextureLabel, m_MainTex);//只显示Texture
        // m_materialEditor.TextureScaleOffsetProperty(m_MainTex);//只显示Offset和Scale



        if (showDetails)
        {
            m_materialEditor.SetDefaultGUIWidths(); //把fieldWidth 的值设为和高度一样
            GUILayout.Space(5);
            m_materialEditor.ShaderProperty(m_MainTex, Styles.mainTextureLabel);
            //GUILayout.Label("U:[CustomData.x]||V:[CustomData.y]");
            m_materialEditor.ShaderProperty(m_UseCustomData1xy, Styles.useCustomData1xy);
            m_materialEditor.ShaderProperty(m_TintColor, Styles.tintColorLabel);
            m_materialEditor.ShaderProperty(m_Glow, Styles.glowLabel);

            m_materialEditor.ShaderProperty(m_UIClip, Styles.uiClipLabel);
            ShowAlphaEnum();

            GUILayout.Space(5);
        }
        else
        {
            
            GUILayout.Space(5);
            m_materialEditor.TexturePropertySingleLine(Styles.mainTextureLabel, m_MainTex); //只显示Texture
            m_materialEditor.TextureScaleOffsetProperty(m_MainTex); //只显示Offset和Scale
            m_materialEditor.ShaderProperty(m_TintColor, Styles.tintColorLabel);
            m_materialEditor.ShaderProperty(m_Glow, Styles.glowLabel);
            ShowAlphaEnum();

            m_materialEditor.ShaderProperty(m_UIClip, Styles.uiClipLabel);
            GUILayout.Space(5);
        }
        if (m_UseAlpha.floatValue == 0)
        {
            material.DisableKeyword("ENABLE_Alpha");
        }
        else
        {
            material.EnableKeyword("ENABLE_Alpha");
        }
        //扩展功能开关
        toolSwitch = GUILayout.Toggle(toolSwitch, Styles.SwitchMode, EditorStyles.boldLabel);
        if (toolSwitch)
        {
            //边缘光模块
            bool usRimlit = material.IsKeywordEnabled("FX_FRESNEL_RIMLIGHT");
            usRimlit = true ? m_UseFresnel.floatValue == 1 : m_UseFresnel.floatValue == 0;
            usRimlit = GUILayout.Toggle(usRimlit, Styles.RimlitMode, EditorStyles.toggle);
            //GUILayout.Label(Styles.RimlitMode, EditorStyles.boldLabel);
            if (usRimlit)
            {
                GUILayout.Space(5);
                material.EnableKeyword("FX_FRESNEL_RIMLIGHT");
                m_UseFresnel.floatValue = 1;
                m_materialEditor.ShaderProperty(m_FresnekStr, Styles.fresnelStrlLabel);
                GUILayout.Space(5);
            }
            else //禁用Keyword，不显示UI
            {
                m_UseFresnel.floatValue = 0;
                material.DisableKeyword("FX_FRESNEL_RIMLIGHT");
            }

            //UV运动模块
            //bool usUVTrans = (m_UseUVTrans.floatValue == 0)?true:false;
            bool usUVTrans = material.IsKeywordEnabled("FX_UVTRANSFORM");
            usUVTrans = true ? m_UseUVTrans.floatValue == 1 : m_UseUVTrans.floatValue == 0;
            usUVTrans = GUILayout.Toggle(usUVTrans, Styles.UVTransMode, EditorStyles.toggle);

            if (usUVTrans)
            {
                GUILayout.Space(5);
                material.EnableKeyword("FX_UVTRANSFORM");
                m_UseUVTrans.floatValue = 1;
                m_materialEditor.ShaderProperty(m_USpeed, Styles.uvTransUlLabel);
                m_materialEditor.ShaderProperty(m_VSpeed, Styles.uvTransVlLabel);
                GUILayout.Space(5);
            }
            else //禁用Keyword，不显示UI
            {
                m_UseUVTrans.floatValue = 0;
                material.DisableKeyword("FX_UVTRANSFORM");
            }

            //扰动模块
            bool useDistort = material.IsKeywordEnabled("FX_DISTORT");

            useDistort = true ? m_UseDistort.floatValue == 1 : m_UseDistort.floatValue == 0;

            useDistort = GUILayout.Toggle(useDistort, Styles.DistortMode, EditorStyles.toggle);


            if (useDistort)
            {
                m_materialEditor.SetDefaultGUIWidths(); //把fieldWidth 的值设为和高度一样
                GUILayout.Space(5);
                material.EnableKeyword("FX_DISTORT");
                m_UseDistort.floatValue = 1;
                m_materialEditor.ShaderProperty(m_DistortTex, Styles.distortMapLabel);
                m_materialEditor.ShaderProperty(m_DistortStr, Styles.distortStrLabel);
                m_materialEditor.ShaderProperty(m_DistortUSpeed, Styles.uvDistortUlLabel);
                m_materialEditor.ShaderProperty(m_DistortVSpeed, Styles.uvDistortVlLabel);
                GUILayout.Space(5);
            }
            else //禁用Keyword，不显示UI
            {
                m_UseDistort.floatValue = 0;
                material.DisableKeyword("FX_DISTORT");
            }

            //溶解模块
            bool useDissolve = material.IsKeywordEnabled("FX_DISSOLVE");
            useDissolve = true ? m_UseDissolve.floatValue == 1 : m_UseDissolve.floatValue == 0;
            useDissolve = GUILayout.Toggle(useDissolve, Styles.DissolveMode, EditorStyles.toggle);

            if (useDissolve)
            {
                m_materialEditor.SetDefaultGUIWidths(); //把fieldWidth 的值设为和高度一样
                GUILayout.Space(5);
                material.EnableKeyword("FX_DISSOLVE");
                m_UseDissolve.floatValue = 1;
                m_materialEditor.ShaderProperty(m_DissolveTex, Styles.dissolveMapLabel);
                m_materialEditor.ShaderProperty(m_DissolveStr, Styles.dissolveStrLabel);
                m_materialEditor.ShaderProperty(m_DissolveRangeUseCustomData, Styles.useDissolveCustomData1Label);
                m_materialEditor.ShaderProperty(m_DissolveRange, Styles.dissolveRangeLabel);
                m_materialEditor.ShaderProperty(m_DissolveRimlit, Styles.dissolveRimLabel);
                GUILayout.Space(5);
            }
            else //禁用Keyword，不显示UI
            {
                m_UseDissolve.floatValue = 0;
                material.DisableKeyword("FX_DISSOLVE");
            }

            //遮罩模块
            bool useMask = material.IsKeywordEnabled("FX_MASK");
            useMask = true ? m_UseMask.floatValue == 1 : m_UseMask.floatValue == 0;
            useMask = GUILayout.Toggle(useMask, Styles.MaskMode, EditorStyles.toggle);

            if (useMask)
            {
                m_materialEditor.SetDefaultGUIWidths(); //把fieldWidth 的值设为和高度一样
                GUILayout.Space(5);
                material.EnableKeyword("FX_MASK");
                m_UseMask.floatValue = 1;

                
              
                bool useLux;
                useLux = true ? m_UseLux.floatValue == 1 : m_UseLux.floatValue == 0;
                useLux = GUILayout.Toggle(useLux, Styles.useMaskGlowLabel, EditorStyles.toggle);
                if (useLux)
                {
                    m_UseLux.floatValue = 1;
                }
                else
                {
                    m_UseLux.floatValue = 0;
                }
                m_materialEditor.ShaderProperty(m_UseCustomDataMaskUV, Styles.useCustomDataMaskUV);
                m_materialEditor.ShaderProperty(m_Lux, Styles.MaskGlowLabel);
                m_materialEditor.ShaderProperty(m_MaskTex, Styles.maskMapLabel);
                m_materialEditor.ShaderProperty(m_MaskUSpeed, Styles.uvMaskUlLabel);
                m_materialEditor.ShaderProperty(m_MaskVSpeed, Styles.uvMaskVlLabel);
                GUILayout.Space(5);
            }
            else //禁用Keyword，不显示UI
            {
                m_UseMask.floatValue = 0;
                material.DisableKeyword("FX_MASK");
            }
        }
        else
        {
        }

        //渲染模式开关
        RendererSwitch = GUILayout.Toggle(RendererSwitch, Styles.RenderingMode, EditorStyles.boldLabel);
        if (RendererSwitch)
        {
            //Culling Mode表面剔除
            CullingMode cullingMode = (CullingMode) m_CullingMode.floatValue;
            EditorGUI.BeginChangeCheck();
            cullingMode = (CullingMode) EditorGUILayout.Popup(Styles.CullingMode, (int) cullingMode, Styles.CullNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_materialEditor.RegisterPropertyChangeUndo("Culling Switch");
                switch (cullingMode)
                {
                    case CullingMode.双面渲染:
                        m_CullingMode.floatValue = (float) UnityEngine.Rendering.CullMode.Off;
                        break;
                    case CullingMode.渲染背面:
                        m_CullingMode.floatValue = (float) UnityEngine.Rendering.CullMode.Front;
                        break;
                    case CullingMode.渲染前面:
                        m_CullingMode.floatValue = (float) UnityEngine.Rendering.CullMode.Back;
                        break;
                }
            }

            //ZWrite Mode深度写入
            ZWriteMode zWriteMode = (ZWriteMode) m_ZWriteMode.floatValue;
            EditorGUI.BeginChangeCheck();
            zWriteMode = (ZWriteMode) EditorGUILayout.Popup(Styles.ZWriteMode, (int) zWriteMode, Styles.ZWriteNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_materialEditor.RegisterPropertyChangeUndo("ZWrite Switch");
                m_ZWriteMode.floatValue = (float) zWriteMode;
            }

            //ZTest Mode深度写入

            //EditorGUI.BeginChangeCheck();
            ZTestMode zTestMode;
            if (m_ZTestMode.floatValue == (float) UnityEngine.Rendering.CompareFunction.Always)
            {
                zTestMode = ZTestMode.处于最上层;
            }
            else
            {
                zTestMode = ZTestMode.处于真实位置;
            }

            zTestMode = (ZTestMode) EditorGUILayout.Popup(Styles.ZTestMode, (int) zTestMode, Styles.ZTestNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_materialEditor.RegisterPropertyChangeUndo("ZTest Switch");
                switch (zTestMode)
                {
                    case ZTestMode.处于最上层:
                        m_ZTestMode.floatValue = (float) UnityEngine.Rendering.CompareFunction.Always;
                        break;
                    case ZTestMode.处于真实位置:
                        m_ZTestMode.floatValue = (float) UnityEngine.Rendering.CompareFunction.LessEqual;
                        break;
                }
            }
        }
        else
        {
        }

        EditorGUILayout.Space();
        GUILayout.Label("Render Queue", EditorStyles.boldLabel);
        m_materialEditor.RenderQueueField();

        EditorGUILayout.Space();
    }

    public void ShowAlphaEnum()
    {
        AlphaChannel alphaChannel;
        if (m_UseAlpha.floatValue == 1)
        {
            if (m_AlphaChannel.vectorValue.x == 1)
            {
                alphaChannel = AlphaChannel.R通道;
            }
            else if (m_AlphaChannel.vectorValue.y == 1)
            {
                alphaChannel = AlphaChannel.G通道;
            }
            else if (m_AlphaChannel.vectorValue.z == 1)
            {
                alphaChannel = AlphaChannel.B通道;
            }
            else
            {
                alphaChannel = AlphaChannel.A通道;
            }
        }
        else
        {
            alphaChannel = AlphaChannel.不使用;
        }

        EditorGUI.BeginChangeCheck();
        alphaChannel = (AlphaChannel) EditorGUILayout.Popup(Styles.AChannelMode, (int) alphaChannel, Styles.AlphaNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_materialEditor.RegisterPropertyChangeUndo("Use Alpha Channel");
            m_materialEditor.RegisterPropertyChangeUndo("MainTex Alpha Channel");

            switch (alphaChannel)
            {
                case AlphaChannel.R通道:
                    m_UseAlpha.floatValue = 1;
                    m_AlphaChannel.vectorValue = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
                    break;
                case AlphaChannel.G通道:
                    m_UseAlpha.floatValue = 1;
                    m_AlphaChannel.vectorValue = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
                    break;
                case AlphaChannel.B通道:
                    m_UseAlpha.floatValue = 1;
                    m_AlphaChannel.vectorValue = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
                    break;
                case AlphaChannel.A通道:
                    m_UseAlpha.floatValue = 1;
                    m_AlphaChannel.vectorValue = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                    break;
                case AlphaChannel.不使用:
                    m_UseAlpha.floatValue = 0;
                    break;
            }
        }
    }
}