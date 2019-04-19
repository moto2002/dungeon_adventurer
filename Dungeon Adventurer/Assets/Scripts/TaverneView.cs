using UnityEngine;

public class TaverneView : View {
    [SerializeField] TaverneCharacter prefab;
    [SerializeField] Transform container;

    const int HeroCount = 4;

    public override void AfterShow() {

        for (int i = 0; i < HeroCount; i++) {
            var character = CharacterCreator.CreateHero();
            var entry = Instantiate(prefab);

            entry.transform.SetParent(container);
            entry.SetData(character);
        }
    }
}
