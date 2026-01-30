
using UnityEngine;

public interface IState
{
    void Enter();
    void Exit();
    void Tick(); // gọi mỗi frame từ chủ
}
