using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class FeedbackGuessItem : MonoBehaviour
    {
        [Header("Backgrounds")]
        [SerializeField] private Image _directorBg;
        [SerializeField] private Image _actorsBg;
        [SerializeField] private Image _genresBg;
        [SerializeField] private Image _yearBg;
        
        [Header("Feedback Texts")]
        [SerializeField] private TMP_Text _directorText;
        [SerializeField] private TMP_Text _actorsText;
        [SerializeField] private TMP_Text _genresText;
        [SerializeField] private TMP_Text _yearText;
        
        [Header("Background Color Assets")]
        [SerializeField] private Sprite _greenImage;
        [SerializeField] private Sprite _orangeImage;
        [SerializeField] private Sprite _redPlainImage;
        [SerializeField] private Sprite _redArrowImage;


        public void Setup(GuessResultModel model)
        {
            _directorText.text = model.DirectorName;
            _actorsText.text = model.ActorsText;
            _genresText.text = model.GenresText;
            _yearText.text = model.YearText;
            
            SetupVisuals(_directorBg, model.DirectorColor, false, false);
            SetupVisuals(_actorsBg, model.ActorsColor, false, false);
            SetupVisuals(_genresBg, model.GenresColor, false, false);
            SetupVisuals(_yearBg, model.YearColor, true, model.RotateYearArrow);
            
            SpinAndPopAnimation().Forget();
        }

        private async UniTask SpinAndPopAnimation()
        {
            await DoSpinAndScale(_directorBg.rectTransform);
            await DoSpinAndScale(_actorsBg.rectTransform);
            await DoSpinAndScale(_genresBg.rectTransform);
            await DoSpinAndScale(_yearBg.rectTransform);
        }

        private async UniTask DoSpinAndScale(RectTransform target)
        {
            const float duration = 0.4f;

            var scaleTask = target.DOScale(Vector3.one, duration)
                .SetEase(Ease.OutBack) 
                .AsyncWaitForCompletion()
                .AsUniTask();
            
            var rotateTask = target.DORotate(new Vector3(0f, 360f, 0f), duration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion()
                .AsUniTask();

            await UniTask.WhenAll(scaleTask, rotateTask);
        }

        private void SetupVisuals(Image bg, FeedbackColor color, bool isYearBox, bool rotateArrow)
        {
            switch (color)
            {
                case FeedbackColor.Green:
                    bg.sprite = _greenImage;
                    break;
                
                case FeedbackColor.Orange:
                    bg.sprite = _orangeImage;
                    break;
                
                case FeedbackColor.Red:
                    if (isYearBox)
                    {
                        bg.sprite = _redArrowImage;

                        if (rotateArrow)
                        {
                            bg.transform.localRotation = Quaternion.Euler(0, 0, 180);

                            if (bg.transform.childCount > 0)
                            {
                                bg.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 180);   
                            }
                        }
                    }
                    else
                    {
                        bg.sprite = _redPlainImage;
                    }
                    break;
            }
        }
    }
}