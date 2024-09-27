namespace CoreAttack
{
    using UnityEngine.Events;

    public interface IHealthOwner
    {
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
        public UnityAction<float> OnHealthUpdated { get; set; }
    }
}