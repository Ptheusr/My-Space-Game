using NUnit.Framework;
using System;
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

	public float rangeDecay; //


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
		ImpactWave, //Sóng xung kích, hỏa lực cao, lượng đạn thấp, tốc độ sát thương trung bình, vùng sát thương lớn
		Melee, //Vũ khí cận chiến, sát thương cao, không cần đạn, tốc độ sát thương thấp đến cao.

		MonsterStrike, //đòn tấn công cận chiến của quái vật
		MonsterRangedStrike //đòn tấn công tầm xa của quái vật
    }
	public WeaponType weaponType;

	public void SetDamageBase(float amount)
	{
		if (amount >= 1)
		{
			damageBase = amount;
		}
		else damageBase = 1;
	}
	
	public void SetDamageInterval(float amount)
	{
		if (amount >= 0.2)
		{
			damageInterval = amount;
		}
		else damageInterval = 0.2f;
	}



	public void SetAoeState(bool state, float amount, int range)
	{
		if (state)
		{
			AOEdamage = true;
			if (amount > 0) { damageAOEbase = amount; } else { damageAOEbase = 1; }
			if (range >= 1) { rangeAOE = range; } else { rangeAOE = 1; }
		}
		else { AOEdamage = false; damageAOEbase = 0; rangeAOE = 0; }
	}

	public void SetRangeDecay(float decay)
	{
		if (decay <= 0.99990f)
		{
			rangeDecay = decay;
		}
		else rangeDecay = 0;
	}

	public void SetAmmunitionState(bool state, int amount)
	{
			if (state)
			{
				canHaveAmmunition = true;
				if (amount >= 1)
				{
					ammunitionBase = amount;
				}
				else ammunitionBase = 100;
			}
			else { canHaveAmmunition = false; ammunitionBase = 0; }
    }
	
	public void SetReloadState(bool state, int count, float interval)
	{
			if (state)
			{
				needReload = true;

				if (count >= 1) { damageCount = count; }
				else damageCount = 1;

				if (interval >= 1.5f) { reloadInterval = interval; }
				else reloadInterval = 1.5f;
			}
			else
			{
				needReload = false;
				damageCount = 0;
				reloadInterval = 0;
			}
    }

	public void SetDamageType(string type)
	{
		switch (type)
		{
			case "Heat": damageType = DamageType.Heat; break;
			case "Energy": damageType = DamageType.Energy; break;
			case "Bio": damageType= DamageType.Bio; break;
			default: damageType = DamageType.Kinetic;
				break;
		}
	}
	
	public void SetWeaponType(string type) 
	{
        weaponType = type switch
        {
            "AutoCannon" => WeaponType.AutoCannon,
            "AntiMaterialCannon" => WeaponType.AntiMaterialCannon,
            "Laser" => WeaponType.Laser,
            "ImpactWave" => WeaponType.ImpactWave,
            "Melee" => WeaponType.Melee,
            "MonsterStrike" => WeaponType.MonsterStrike,
            "MonsterRangedStrike" => WeaponType.MonsterRangedStrike,
            _ => WeaponType.SiegeCannon,
        };
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
		string weaponType = UnityEngine.Random.Range(1,7) switch
        {
            2 => WeaponType.AutoCannon.ToString(),
            3 => WeaponType.AntiMaterialCannon.ToString(),
            4 => WeaponType.Laser.ToString(),
            5 => WeaponType.ImpactWave.ToString(),
            6 => WeaponType.Melee.ToString(),
            _ => WeaponType.SiegeCannon.ToString(),
        };
		weapon.SetWeaponType(weaponType);

		//Loại sát thương
        string damageType = UnityEngine.Random.Range(1, 5) switch
        {
            2 => DamageType.Energy.ToString(),
            3 => DamageType.Heat.ToString(),
            4 => DamageType.Bio.ToString(),
            _ => DamageType.Kinetic.ToString(),
        };
		weapon.SetDamageType(damageType);

		//Phần Reload
		bool needReload = weapon.weaponType != WeaponType.Melee && UnityEngine.Random.value >= 0.5f;
		
		int damageCount;
		float loadInterval;
        if (needReload)
		{
			damageCount = weapon.weaponType switch
			{
				WeaponType.AutoCannon => 100 + (int)UnityEngine.Random.Range((powerLevel * 0.23f) * 5, (powerLevel * 0.45f) * 8),
				WeaponType.AntiMaterialCannon => 1 + (int)UnityEngine.Random.Range(powerLevel * 0.005f, powerLevel * 0.01f),
				WeaponType.Laser => 50 + (int)UnityEngine.Random.Range((powerLevel * 0.2f) * 3, (powerLevel * 0.38f) * 6),
				WeaponType.ImpactWave => 5 + (int)UnityEngine.Random.Range(powerLevel * 0.05f, powerLevel * 0.08f),
				_ => 10 + (int)UnityEngine.Random.Range((powerLevel * 0.07f) * 2, (powerLevel * 0.12f) * 3)
			};

            loadInterval = weapon.weaponType switch
            {
                WeaponType.AutoCannon => 8f - UnityEngine.Random.Range(powerLevel * 0.006f, powerLevel * 0.008f) ,
                WeaponType.AntiMaterialCannon => 20f - UnityEngine.Random.Range(powerLevel * 0.002f, powerLevel * 0.0035f),
                WeaponType.Laser => 6f - UnityEngine.Random.Range(powerLevel * 0.0037f, powerLevel * 0.0055f),
                WeaponType.ImpactWave => 13f - UnityEngine.Random.Range(powerLevel * 0.0024f, powerLevel * 0.0032f),
                _ => 10f - UnityEngine.Random.Range(powerLevel * 0.0032f, powerLevel * 0.006f)
            };
        }
		else { damageCount = 0; loadInterval = 0; }
		weapon.SetReloadState(needReload,damageCount,loadInterval);

        //Phần Ammunition
        bool haveAmmunition = weapon.weaponType != WeaponType.Melee && UnityEngine.Random.value >= 0.5f;

        int ammunitionCount;
        if (needReload)
        {
            ammunitionCount = weapon.weaponType switch
            {
                WeaponType.AutoCannon => 1000 + (int)UnityEngine.Random.Range((powerLevel * 0.5f) * 5, (powerLevel * 0.8f) * 7),
                WeaponType.AntiMaterialCannon => 5 + (int)UnityEngine.Random.Range(powerLevel * 0.008f, powerLevel * 0.015f),
                WeaponType.Laser => 500 + (int)UnityEngine.Random.Range((powerLevel * 0.35f) * 4, (powerLevel * 0.58f) * 8),
                WeaponType.ImpactWave => 50 + (int)UnityEngine.Random.Range(powerLevel * 0.1f, powerLevel * 0.2f),
                _ => 100 + (int)UnityEngine.Random.Range((powerLevel * 0.14f) * 3, (powerLevel * 0.22f) * 5)
            };
        }
        else { ammunitionCount = 0; }
		weapon.SetAmmunitionState(haveAmmunition, ammunitionCount);

		//Phần Range decay
		float decay = weapon.weaponType switch
        {
            WeaponType.AutoCannon => UnityEngine.Random.Range(0.3f, 0.81f),
            WeaponType.AntiMaterialCannon => UnityEngine.Random.Range(0.011f, 0.041f),
            WeaponType.Laser => UnityEngine.Random.Range(0.5f, 0.91f),
            WeaponType.ImpactWave => UnityEngine.Random.Range(0.6f, 0.951f),
            _ => UnityEngine.Random.Range(0.21f, 0.41f)
        };
		weapon.SetRangeDecay(decay);

		//Phần AOE State
        return weapon;
	}
}
