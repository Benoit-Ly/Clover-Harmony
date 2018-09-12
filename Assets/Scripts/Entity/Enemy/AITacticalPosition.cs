using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITacticalPosition {

    #region Enums
    public enum Space
    {
        BOTH = 0,
        LEFT,
        RIGHT
    }
    #endregion

    #region Vars
    Player m_Target;                    public Player Target { get { return m_Target; } set { m_Target = value; } }
    List<AIController> m_LeftAI;
    List<AIController> m_RightAI;
    #endregion

    public AITacticalPosition(Player target)
    {
        m_Target = target;
        m_LeftAI = new List<AIController>();
        m_RightAI = new List<AIController>();
    }

    public Space GetFreeSpace()
    {
        int leftCount = m_LeftAI.Count;
        int rightCount = m_RightAI.Count;

        if (leftCount > rightCount)
            return Space.RIGHT;
        else if (leftCount < rightCount)
            return Space.LEFT;
        else
            return Space.BOTH;
    }

    public void AddAI(AIController ai, Space space)
    {
        switch(space)
        {
            case Space.LEFT:
                {
                    m_LeftAI.Add(ai);
                    break;
                }

            case Space.RIGHT:
                {
                    m_RightAI.Add(ai);
                    break;
                }

            default: break;
        }
    }

    public void RemoveAI(AIController ai)
    {
        m_LeftAI.Remove(ai);
        m_RightAI.Remove(ai);
    }

    public int GetAggroNumber()
    {
        return m_LeftAI.Count + m_RightAI.Count;
    }

    public void RebalanceAggro()
    {
        int leftCount = m_LeftAI.Count;
        int rightCount = m_RightAI.Count;
        int difference = leftCount - rightCount;

        if (difference > 2)
            MoveAggro(ref m_LeftAI, ref m_RightAI, difference / 2);
        else if (difference < -2)
            MoveAggro(ref m_RightAI, ref m_LeftAI, difference / 2);
    }

    void MoveAggro(ref List<AIController> source, ref List<AIController> destination, int nb)
    {
        for (int i = 0; i < nb; i++)
        {
            destination.Add(source[0]);
            source.RemoveAt(0);
        }
    }
}
