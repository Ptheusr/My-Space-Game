using System.Collections.Generic;
using UnityEngine;

public static class CharacterStorage
{
    public static bool firstime = true;
    public static CharacterData motherShip;
    public static List<CharacterData> Ally_Unit;
    public static List<CharacterData> Enermy_Unit;
}
public class CharacterData
{
    public string ID; //id của đơn vị

    public float healthBase; //máu của đơn vị

    public bool canRegen; //bool dành cho quái vật, quái vật có thể hồi máu.
    public float healthRegenBase; //hồi máu của quái vật. 1 lần mỗi 5 giây

    public int armorBase;  //giáp của đơn vị. Theo cơ chế 2 từ 1 với điểm sát thương. nhưng số lượng trừ không được quá 90% số sát thương gốc.

    public bool canHaveShield; //bool dành cho quái vật, vì quái vật không có khiên.
    public float shieldBase; //khiên của đơn vị. Nhận sát thương thay cho máu, nhưng không tính cơ chế giáp. thay vào đó, cố định miễn 10% sát thương gốc.
    public float shieldRechargeBase; //lượng hồi phục lại khiên sau 2 giây không nhận sát thương. 1 lần hồi mỗi 2 giây

    public float speedBase; //tốc độ di chuyển của đơn vị. tính trên số ô mỗi giây.

    #region A Unit
    public enum A_UnitType //Kiểu dạng ngoại hình hoặc phân loại ngoại hình của đơn vị đồng minh. ví dụ: tank, tàu, máy bay, cơ giáp bộ binh
    {
        MobileSuit, //giáp cơ động 1 người hạng nhẹ
        Drone, // vật thể bay không người lái kích cỡ nhỏ

        FlyingObject, //Loại khác
        
        HoverTank, //Tank cơ động cỡ nhỏ
        HoverShip, //Tàu cơ động cỡ nhỏ
        
        BattleTank, //Tank chiến đấu cỡ trung
        BattleShip, //Tàu chiến đấu cỡ trung
        BattleMobileSuit, //giáp cơ động 1 người hạng trung

        WarTank, //Tank chiến tranh cỡ lớn
        WarShip, //Tàu chiến tranh cỡ lớn
        WarMechanicalSuit, //giáp cơ động 1 người hạng nặng

        MotherShip //Tàu mẹ
    }
    public A_UnitType unitType_A;

    public enum A_UnitClass //phân loại nghề của đơn vị đồng minh
    {
        Dealer, //Cân bằng về khả năng tác chiến và lượng hỏa lực. 
        Shielder, //Khả năng chống chịu hỏa lực cao.
        Supplier, //Nạp lại đạn dược cho các đơn vị ở gần.
        Miner, //Các đơn vị khai thác.
        Scout, //Độ cơ động cao, lượng hỏa lực trung bình thấp, khoảng cách tác chiến gần
        Striker, //Hỏa lực khủng bố, khoảng cách tác chiến cực xa.
        MotherShip //Tàu mẹ
    }
    public A_UnitClass unitClass_A;
    #endregion

    #region E_Unit
    public enum E_UnitType //Kiểu dạng ngoại hình hoặc phân loại ngoại hình của đơn vị kẻ thù
    {
       Beast, //Hình dạng động vật, quái thú
       Manlike, //Hình dạng giống các loại linh trưởng, con người
       EnergyForm, //Dạng năng lượng, hoặc có lõi năng lượng là thân thể chính
       Unknow //loại khác
    }
    public E_UnitType unitType_E;

    public enum E_UnitTier //phân loại nghề của đơn vị kẻ thù
    {
        Lesser, // cá thể chưa trưởng thành, các chỉ số thấp
        Mature, // cá thể trưởng thành, các chỉ số ở mức trung bình
        Evolved, // cá thể đã trải qua tiến hóa, các chỉ số ở mức trung bình cao
        Dominant, // cá thể thống trị, các chỉ số ở mức cao - rất cao
        Terror // Boss
    }
    public E_UnitTier unitClass_E;
    #endregion


}
