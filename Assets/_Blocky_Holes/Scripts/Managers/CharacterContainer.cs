using UnityEngine;

namespace ClawbearGames
{
    public class CharacterContainer : MonoBehaviour
    {
        public int SelectedCharacterIndex { get { return PlayerDataHandler.GetSelectedCharacterIndex(); } }
        public CharacterInforController[] CharacterInforControllers { get { return characterInforControllers; } }
        [SerializeField] private CharacterInforController[] characterInforControllers = null;
    }

}
