using Unity.Entities;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    private PlayerInput playerInput;

    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton(out InputComponents input))
        {
            EntityManager.CreateEntity(typeof(InputComponents));
        }

        playerInput = new PlayerInput();
        playerInput.Enable();
    }

    protected override void OnUpdate()
    {
        Vector2 moveVector = playerInput.Player.Move.ReadValue<Vector2>();
        Vector2 lookDirection = playerInput.Player.Look.ReadValue<Vector2>();
        float scrolling = playerInput.Player.Zoom.ReadValue<float>();
        float jumping = playerInput.Player.Jump.ReadValue<float>();

        SystemAPI.SetSingleton(new InputComponents
        {
            Movement = moveVector,
            LookDirection = lookDirection,
            Scrolling = scrolling,
            Jumping = jumping,
        });
    }
}
