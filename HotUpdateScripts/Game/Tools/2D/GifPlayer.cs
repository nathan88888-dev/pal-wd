using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GifPlayer : MonoBehaviour
{
    public List<Sprite> _gifTexList;

    private Image targetImage;

    private void Awake()
    {
        // 自动获取 RawImage
        targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        PlayGif(_gifTexList, _gifTexList.Count).Forget();
    }

    async UniTaskVoid PlayGif(List<Sprite> gifTexList, int loopCount) {

        if (gifTexList != null)
        {
            int index = 0;
            while (true)
            {
                targetImage.sprite = gifTexList[index++];
                if (index >= loopCount)
                {
                    index = 0;
                }
                await UniTask.Delay(330);
            }
        }
        else
        {
            Debug.LogError("Gif texture get error.");
        }
    }
}
