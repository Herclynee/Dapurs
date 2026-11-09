using UnityEngine;

public interface IState
{
    void Enter();                 // dipanggil sekali saat masuk state
    void Tick(float deltaTime);   // dipanggil tiap frame selama state aktif
    void Exit();                  // dipanggil sekali saat keluar state
}