using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon
{
	public string wID; // id của vũ khí

	public float damageBase; //sát thương cơ bản của vũ khí. tính trên mỗi lần gây sát thương
	public float damageInterval; //delay giữa 2 lần gây sát thương kế nhau. Tách biệt với Reload.

	public bool canHaveAmmunition; //bool cho phép vũ khí có thể có đạn dược dự trữ. dành cho vũ khí của đồng minh
	public int ammunitionBase; //lượng đạn dược dự trữ, tính trên số lượt tấn công

	public bool needReload; //có cần delay sau 1 số lần gây sát thương nhất định không.
	public int damageCount; // số lần sát thương trước khi cần reload
	public float reloadInterval; //thời gian reload.

	public bool AOEdamage; //vũ khí có gây ra sát thương phạm vi không.
	public float damageAOEbase; //sát thương phạm vi của vũ khí, tách biệt với sát thương thông thường. mục tiêu của đòn đánh có sát thương AOE sẽ chịu cả damage cơ bản và AOE damage
	public int rangeAOE; //phạm vi chịu damageAOE, theo công thức x,y = rangeAOE. với mục tiêu gốc là 0,0 và phạm vi sẽ từ 0,0 đến +x,+y và -x,-y.

	public float rangeDecay; //tỷ lệ giảm sát thương khi ngoài tầm tấn công hiệu quả của Đơn vị.
	public float pointBlankBonus = 1.5f; // sát thương tăng thêm khi mục tiêu ở ô liền kề. Chỉ dành cho loại Shrapnel. Cố định 1.5f

    public enum DamageType //loại sát thương, không ảnh hưởng đến các cơ chế giáp, nhưng liên quan đến các phương thức tấn công và khởi tạo vũ khí
	{
		Kinetic, //Sát thương va chạm vật lí
		Heat,	 //Sát thương nhiệt
		Energy,	 //Sát thương năng lượng
		Bio		 //Sát thương sinh học
	}
	public DamageType damageType;

	public enum WeaponType //Loại vũ khí
	{
        SiegeCannon, //Pháo cơ bản, hỏa lực trung bình khá, lượng đạn trung bình, tốc độ sát thương trung bình
		AutoCannon, //Súng máy, hỏa lực yếu, lượng đạn lớn, tốc độ sát thương cao
		AntiMaterialCannon, //Pháo hạng nặng, Hỏa lực cực lớn, lượng đạn thấp, tốc độ sát thương thấp, vùng sát thương nhỏ
		Laser, //Súng năng lượng, hỏa lực trung bình yếu, lượng đạn trung bình, tốc độ sát thương rất cao
		HeavyBomber, //Sóng xung kích, hỏa lực cao, lượng đạn thấp, tốc độ sát thương trung bình, vùng sát thương lớn
		Shrapnel, //Vũ khí tầm gần, bắn chùm đạn vào mục tiêu. sát thương mạnh ở tầm gần nhưng giảm rõ rệt ở xa.

		MonsterStrike, //đòn tấn công cận chiến của quái vật
		MonsterRangedStrike //đòn tấn công tầm xa của quái vật
    }
	public WeaponType weaponType;
	
    //Weapon Type
	public WeaponType SetWeaponType(bool random, string value) 
	{
		WeaponType Type;
		if (random)
		{
            Type = UnityEngine.Random.Range(1, 7) switch
            {
                2 => WeaponType.AutoCannon,
                3 => WeaponType.AntiMaterialCannon,
                4 => WeaponType.Laser,
                5 => WeaponType.HeavyBomber,
                6 => WeaponType.Shrapnel,
                _ => WeaponType.SiegeCannon,
            };
        }
		else
		{
			Type = value switch
			{
				"AutoCannon" => WeaponType.AutoCannon,
				"AntiMaterialCannon" => WeaponType.AntiMaterialCannon,
				"Laser" => WeaponType.Laser,
				"HeavyBomber" => WeaponType.HeavyBomber,
				"Shrapnel" => WeaponType.Shrapnel,
				"MonsterStrike" => WeaponType.MonsterStrike,
				"MonsterRangedStrike" => WeaponType.MonsterRangedStrike,
				_ => WeaponType.SiegeCannon,
			};
		}
		return Type;
    }
    
    //Damage Type
    public DamageType SetDamageType(bool random, string value)
    {
        DamageType Type;
        if (random)
        {
            Type = UnityEngine.Random.Range(1, 5) switch
            {
                2 => DamageType.Energy,
                3 => DamageType.Heat,
                4 => DamageType.Bio,
                _ => DamageType.Kinetic,
            };
        }
        else
        {
            Type = value switch
            {
                "Energy" => DamageType.Energy,
                "Heat" => DamageType.Heat,
                "Bio" => DamageType.Bio,
                _ => DamageType.Kinetic,
            };
        }
        return Type;
    }

    //Reload State
    public void SetReloadState(Weapon origin, bool Random, int powerLevel, bool reloadState, int magCap, float loadInterval)
    {
        if (Random)
        {
            bool needReload = UnityEngine.Random.value >= 0.5f;

            if (needReload)
            {
                origin.damageCount = origin.weaponType switch
                {
                    WeaponType.AutoCannon => 100 + (int)UnityEngine.Random.Range((powerLevel * 0.23f) * 5, (powerLevel * 0.45f) * 8),
                    WeaponType.AntiMaterialCannon => 1 + (int)UnityEngine.Random.Range(powerLevel * 0.005f, powerLevel * 0.01f),
                    WeaponType.Laser => 50 + (int)UnityEngine.Random.Range((powerLevel * 0.2f) * 3, (powerLevel * 0.38f) * 6),
                    WeaponType.HeavyBomber => 5 + (int)UnityEngine.Random.Range(powerLevel * 0.05f, powerLevel * 0.08f),
                    WeaponType.Shrapnel => 8 + (int)UnityEngine.Random.Range(powerLevel * 0.065f, powerLevel * 0.095f),
                    _ => 15 + (int)UnityEngine.Random.Range((powerLevel * 0.07f) * 2, (powerLevel * 0.12f) * 3)
                };

                origin.reloadInterval = origin.weaponType switch
                {
                    WeaponType.AutoCannon => 8f - UnityEngine.Random.Range(powerLevel * 0.006f, powerLevel * 0.008f),
                    WeaponType.AntiMaterialCannon => 20f - UnityEngine.Random.Range(powerLevel * 0.002f, powerLevel * 0.0035f),
                    WeaponType.Laser => 6f - UnityEngine.Random.Range(powerLevel * 0.0037f, powerLevel * 0.0055f),
                    WeaponType.HeavyBomber => 13f - UnityEngine.Random.Range(powerLevel * 0.0024f, powerLevel * 0.0032f),
                    WeaponType.Shrapnel => 11.5f - UnityEngine.Random.Range(powerLevel * 0.0018f, powerLevel * 0.00265f),
                    _ => 10f - UnityEngine.Random.Range(powerLevel * 0.0032f, powerLevel * 0.006f)
                };
            }
            else { origin.damageCount = 0; origin.reloadInterval = 0; }
        }
        else
        {
            needReload = reloadState;
            if (needReload) 
            {
                if (magCap >= 1)
                {
                    origin.damageCount = magCap;
                }
                else origin.damageCount = 1;

                if(loadInterval >= 0.5f)
                {
                    origin.reloadInterval = loadInterval;
                }else origin.reloadInterval = 1f;
            }else { origin.damageCount = 0; origin.reloadInterval = 0; }
        }
        
    }

    //Set AmmunitionState
    public void SetAmmunitionState(Weapon origin, bool random, int powerLevel, bool ammunitionState, int amount )
    {
        if(random)
        {
            bool haveAmmunition = UnityEngine.Random.value <= 0.75f;
            if (haveAmmunition)
            {
                origin.ammunitionBase = origin.weaponType switch
                {
                    WeaponType.AutoCannon => 1000 + (int)UnityEngine.Random.Range((powerLevel * 0.5f) * 5, (powerLevel * 0.8f) * 7),
                    WeaponType.AntiMaterialCannon => 5 + (int)UnityEngine.Random.Range(powerLevel * 0.008f, powerLevel * 0.015f),
                    WeaponType.Laser => 500 + (int)UnityEngine.Random.Range((powerLevel * 0.35f) * 4, (powerLevel * 0.58f) * 8),
                    WeaponType.HeavyBomber => 50 + (int)UnityEngine.Random.Range(powerLevel * 0.1f, powerLevel * 0.2f),
                    WeaponType.Shrapnel => 80 + (int)UnityEngine.Random.Range(powerLevel * 0.12f, powerLevel * 0.23f),
                    _ => 100 + (int)UnityEngine.Random.Range((powerLevel * 0.14f) * 3, (powerLevel * 0.22f) * 5)
                };
            }
            else { origin.ammunitionBase = 0; }
        }
        else
        {
            origin.canHaveAmmunition = ammunitionState;
            if (ammunitionState)
            {
                origin.ammunitionBase = amount >= 1 ? amount : 1;
            }
            else {origin.ammunitionBase = 0;}
        }
    }

    //Range Decay
    public void SetRangeDecay(Weapon origin, bool random, float amount)
    {
        if (random)
        {
            origin.rangeDecay = origin.weaponType switch
            {
                WeaponType.AutoCannon => UnityEngine.Random.Range(0.35f, 0.61f),
                WeaponType.AntiMaterialCannon => UnityEngine.Random.Range(0.011f, 0.051f),
                WeaponType.Laser => UnityEngine.Random.Range(0.5f, 0.81f),
                WeaponType.HeavyBomber => UnityEngine.Random.Range(0.6f, 0.91f),
                WeaponType.Shrapnel => UnityEngine.Random.Range(0.8f, 0.951f),
                _ => UnityEngine.Random.Range(0.21f, 0.41f)
            };
        }
        else 
        {
            if (amount >= 0f && amount <= 1f)
            {
                origin.rangeDecay = amount;
            }
            else origin.rangeDecay = 0.25f;
        }
    }

    //Aoe State
    public void SetAoeState(Weapon origin, bool random, int powerLevel, bool haveAOE, float damage, int range)
    {
        if (random)
        {
            bool AOE;
            if (origin.weaponType != WeaponType.HeavyBomber)
            {
                AOE = UnityEngine.Random.value >= 0.5f;
            }
            else AOE = true;

            if (AOE) 
            {
                origin.damageAOEbase = origin.weaponType switch
                {
                    WeaponType.AutoCannon => 5 + UnityEngine.Random.Range((powerLevel * 0.012f) * 1.05f, (powerLevel * 0.014f) * 1.075f),
                    WeaponType.AntiMaterialCannon => 30 + UnityEngine.Random.Range((powerLevel * 0.02f) * 1.15f, (powerLevel * 0.035f) * 1.25f),
                    WeaponType.Laser => 2.5f + UnityEngine.Random.Range((powerLevel * 0.003f) * 1.01f, (powerLevel * 0.006f) * 1.01f),
                    WeaponType.HeavyBomber => 50 + UnityEngine.Random.Range((powerLevel * 0.03f) * 1.3f, (powerLevel * 0.055f) * 1.5f),
                    WeaponType.Shrapnel => 20 + UnityEngine.Random.Range((powerLevel * 0.012f) * 1.23f, (powerLevel * 0.025f) * 1.35f),
                    _ => 15 + UnityEngine.Random.Range((powerLevel * 0.015f) * 1.1f, (powerLevel * 0.02f) * 1.25f)
                };

                origin.rangeAOE = origin.weaponType switch
                {
                    WeaponType.AutoCannon => UnityEngine.Random.Range(1, 3),
                    WeaponType.AntiMaterialCannon => UnityEngine.Random.Range(1, 4),
                    WeaponType.Laser => 1,
                    WeaponType.HeavyBomber => UnityEngine.Random.Range(2, 6),
                    WeaponType.Shrapnel => UnityEngine.Random.Range(1, 3),
                    _ => UnityEngine.Random.Range(1, 4)
                };
            }
            else { origin.damageAOEbase = 0; origin.rangeAOE = 0; }

        }
    }

    public static Weapon A_WeaponRandomGenerated() //tạo vũ khí ngẫu nhiên chỉ dành cho đồng minh
	{
		Weapon weapon = new Weapon();
		
		float roll = UnityEngine.Random.value;
        int powerLevel = roll switch
        {
            <= 0.5f => UnityEngine.Random.Range(10, 81),
            <= 0.7f => UnityEngine.Random.Range(120, 251),
            <= 0.85f => UnityEngine.Random.Range(300, 501),
            <= 0.95f => UnityEngine.Random.Range(750, 901),
            _ => UnityEngine.Random.Range(1100, 1601)
        };

		//Loại vũ khí
		weapon.weaponType = weapon.SetWeaponType(true, "");

        //Loại sát thương
        weapon.damageType = weapon.SetDamageType(true, "");

        //Phần Reload
        weapon.SetReloadState(weapon, true, powerLevel, false, 0, 0);

        //Phần Ammunition
        weapon.SetAmmunitionState(weapon, true, powerLevel, false, 0);

        //Phần Range decay
        weapon.SetRangeDecay(weapon, true, 0);

		//Phần AOE State
		bool haveAOE;
		if (weapon.weaponType != WeaponType.HeavyBomber)
		{
			haveAOE = UnityEngine.Random.value >= 0.5f;
		}
		else haveAOE = true;
        float count;
        int aoerange;
        if (haveAOE)
        {
            count = weapon.weaponType switch
            {
                WeaponType.AutoCannon => 5 + UnityEngine.Random.Range((powerLevel * 0.012f) * 1.05f, (powerLevel * 0.014f) * 1.075f),
                WeaponType.AntiMaterialCannon => 30 + UnityEngine.Random.Range((powerLevel* 0.02f) * 1.15f, (powerLevel * 0.035f) * 1.25f),
                WeaponType.Laser => 2.5f + UnityEngine.Random.Range((powerLevel * 0.003f) * 1.01f, (powerLevel * 0.006f) * 1.01f),
                WeaponType.HeavyBomber => 50 + UnityEngine.Random.Range((powerLevel * 0.03f) * 1.3f, (powerLevel * 0.055f) * 1.5f),
                _ => 15 + UnityEngine.Random.Range((powerLevel * 0.015f) * 1.1f, (powerLevel * 0.02f) * 1.25f)
            };

            aoerange = weapon.weaponType switch
            {
                WeaponType.AutoCannon => UnityEngine.Random.Range(1, 3),
                WeaponType.AntiMaterialCannon => UnityEngine.Random.Range(1, 4),
                WeaponType.Laser => 1,
                WeaponType.HeavyBomber => UnityEngine.Random.Range(2, 6),
                _ => UnityEngine.Random.Range(1, 4)
            };
        }
        else { count = 0; aoerange = 0; }

        //Phần Damage Interval
        float Damageinterval = weapon.weaponType switch
        {
            WeaponType.AutoCannon => UnityEngine.Random.Range(0.45f, 2.51f),
            WeaponType.AntiMaterialCannon => UnityEngine.Random.Range(10f, 21.1f),
            WeaponType.Laser => UnityEngine.Random.Range(0.25f, 1.01f),
            WeaponType.HeavyBomber => UnityEngine.Random.Range(3.5f, 9.51f),
            _ => UnityEngine.Random.Range(0.75f, 7.1f)
        };

        //Phần Damage
        float damage = weapon.weaponType switch
        {
            WeaponType.AutoCannon =>  3f + UnityEngine.Random.Range((powerLevel/12f) * 1.15f, (powerLevel / 10f) * 1.2f),
            WeaponType.AntiMaterialCannon => 100 + UnityEngine.Random.Range((powerLevel / 6f) * 3.35f, (powerLevel / 5f) * 4.5f),
            WeaponType.Laser => 1 + UnityEngine.Random.Range((powerLevel / 14f) * 1.05f, (powerLevel / 16f) * 1.1f),
            WeaponType.HeavyBomber => 60 + UnityEngine.Random.Range((powerLevel / 10f) * 2f, (powerLevel / 7f) * 3f),
            _ => 40 + UnityEngine.Random.Range((powerLevel / 8f) * 1.5f, (powerLevel / 6f) * 2.5f)
        };

        return weapon;
	}
}
