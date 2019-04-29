public interface IWeaponOwner {
    bool IsAttacking { get; }
    void ReceiveScrap(int scrap);
}