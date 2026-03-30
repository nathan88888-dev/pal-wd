
using UnityEngine;

    public class UISkillPanel : MonoBehaviour
    {
        public GameObject skillTag;
        public GameObject magicTag;
        public GameObject itemTag;

        internal void SetOperType(operType type)
        {
            skillTag.SetActive(type == operType.Skill);
            magicTag.SetActive(type == operType.Magic);
            itemTag.SetActive(type == operType.Item);
        }
    }
