﻿using UnityEngine;
using System.Collections;

public class State : MonoBehaviour {

    public MonsterInfomation monsterInfo;
    public Transform monsterTransform;
    public Rigidbody2D monsterRigidbody2D;
    public Sprite monsterSprite;
    public SpriteRenderer monsterSpriteRenderer;
    public HealthScript health;
	public virtual void UpdateState () {
	}

    public virtual bool checkForCollision()
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(monsterTransform.position, monsterSprite.bounds.size,0,Vector2.zero);

        foreach (RaycastHit2D temp in hit)
        {
            if (temp.collider != null)
            {
                if (temp.collider.gameObject.tag == "Player" && temp.collider.gameObject.GetComponent<HealthScript>() != null)
                {
                    temp.collider.gameObject.GetComponent<HealthScript>().m_health -= monsterInfo.dps;
                    return true;
                }
            }
        }
        return false;
    }
    public virtual bool checkForPlayerInRange()
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(monsterTransform.position, monsterSprite.bounds.size.x * 3, Vector2.zero, 0);

        foreach (RaycastHit2D temp in hit)
        {
            if (temp.collider != null)
            {
                if (temp.collider.gameObject.tag == "Player")
                {
                    return true;
                }
            }
        }
        return false;
    }

}
