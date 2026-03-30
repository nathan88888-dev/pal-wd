
    public static class PathConstant
    {
        public static readonly string UIPrefabPath = "Res/02_UIPrefabs/";

        public static readonly string AtlasSpritePath = "Res/03_AtlasClips/";

        public static readonly string PackScenePath = "01_Scenes_";

        public static readonly string PackPlotPath = "Res/06_Plot/";
        public static readonly string PackMediaPath = "Res/05_Medias/";
        public static readonly string PackModelsPath = "Res/07_Models/";

        public static readonly string ConfigsPath = "Res/95_Config/";


        public static string GetAnimationlPrefabPath(string prefabName)
        {
            return PackModelsPath + prefabName + ".prefab";
        }
        public static string GetMaterialPrefabPath(string prefabName)
        {
            return PackModelsPath + prefabName + ".mat";
        }
        public static string GetModelsPrefabPath(string prefabName) { 
            return PackModelsPath+ prefabName;
        }

        public static string GetMediaPrefabPath(string prefabName)
        {
            return PackMediaPath + prefabName + ".prefab";
        }
        public static string GetUIPrefabPath(string prefabName)
        {
            return UIPrefabPath + prefabName + ".prefab";
        }

        public static string GetAtlasSpritePath(string atlasName, string spriteName)
        {
            //分步concat，优化GC
            return string.Concat(AtlasSpritePath, atlasName, "/" + spriteName + ".png");
        }



    }
