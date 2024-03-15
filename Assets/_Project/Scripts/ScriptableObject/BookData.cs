using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BookData", order = 1)]
public class BookData : ScriptableObject
{
    [Required]
    public string Title;

    [Required]
    public string Author;

    [Required]
    public string Blurb;

    public string Quote;
}