namespace Betauer.StateMachine {
    public enum ProcessMode {
        /// <summary>
        /// <para>The state machine is updated in the <c>_PhysicsProcess</c> callback.</para>
        /// </summary>
        Physics,

        /// <summary>
        /// <para>The state machine is updated int the <c>_Process</c> callback.</para>
        /// </summary>
        Idle,
    }
}