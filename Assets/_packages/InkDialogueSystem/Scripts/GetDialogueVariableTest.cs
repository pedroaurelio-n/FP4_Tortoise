using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDialogueVariableTest : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color bulbasaurColor;
    [SerializeField] private Color charmanderColor;
    [SerializeField] private Color squirtleColor;

    private MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        string pokemonName = ((Ink.Runtime.StringValue)DialogueManager.GetVariableState("pokemon_name")).value;

        switch (pokemonName)
        {
            case "": mesh.material.color = defaultColor; break;
            case "Bulbasaur": mesh.material.color = bulbasaurColor; break;
            case "Charmander": mesh.material.color = charmanderColor; break;
            case "Squirtle": mesh.material.color = squirtleColor; break;
            default: Debug.LogWarning("Pokemon name not handled by switch statement: " + pokemonName); break;
        }
    }
}
