using System;

namespace Betauer.Core.PCG.Examples.ThirdPartyCode.Metazelda.util;

public class MZGenerationFailureException : Exception {
    public MZGenerationFailureException(string message) : base(message) {
    }

    public MZGenerationFailureException(string message, Exception cause) : base(message) {
    }

}