using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Referencias del Panel")]
    public GameObject friendPanel;
    public Button toggleButton;
    public Image buttonIcon;

    [Header("Configuración de Animación")]
    public float animationDuration = 0.3f;
    public Ease animationEase = Ease.OutQuad;

    [Header("Iconos (Opcional)")]
    public Sprite openIcon;
    public Sprite closeIcon;  

    private bool isPanelOpen = true;
    private Vector3 originalPosition;
    private RectTransform panelRectTransform;
    private float panelWidth;

    void Start()
    {
        panelRectTransform = friendPanel.GetComponent<RectTransform>();
        originalPosition = panelRectTransform.anchoredPosition;
        panelWidth = panelRectTransform.rect.width;

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleFriendPanel);
        }

        UpdateButtonIcon();
    }

    public void ToggleFriendPanel()
    {
        if (isPanelOpen)
        {
            CloseFriendPanel();
        }
        else
        {
            OpenFriendPanel();
        }
    }

    public void CloseFriendPanel()
    {
        if (!isPanelOpen) return;

        isPanelOpen = false;

        // Posición de destino (deslizar hacia la izquierda)
        Vector3 targetPosition = originalPosition + new Vector3(-panelWidth, 0, 0);


        panelRectTransform.DOAnchorPos(targetPosition, animationDuration)
            .SetEase(animationEase)
            .OnComplete(() =>
            {
                // Opcional: desactivar el panel cuando esté completamente cerrado
                // friendPanel.SetActive(false);
            });

        UpdateButtonIcon();
    }

    public void OpenFriendPanel()
    {
        if (isPanelOpen) return;

        isPanelOpen = true;

        friendPanel.SetActive(true);

        // Animar el panel de vuelta a su posición original
        panelRectTransform.DOAnchorPos(originalPosition, animationDuration)
            .SetEase(animationEase);

        UpdateButtonIcon();
    }

    private void UpdateButtonIcon()
    {
        if (buttonIcon != null)
        {
            if (isPanelOpen && closeIcon != null)
            {
                buttonIcon.sprite = closeIcon;
            }
            else if (!isPanelOpen && openIcon != null)
            {
                buttonIcon.sprite = openIcon;
            }
        }
    }
    
    void OnDestroy()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(ToggleFriendPanel);
        }
    }

}
