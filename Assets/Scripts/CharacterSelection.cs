using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public Button[] characterButtons;
    private int selectedIndex = -1;

    void Start()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }
    }

    void SelectCharacter(int index)
    {
        selectedIndex = index;
        characterButtons[index].Select();

        Debug.Log("Personaje seleccionado: " + index);
    }
}