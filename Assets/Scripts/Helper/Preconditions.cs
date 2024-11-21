using System;

///// <summary>
///// https://github.com/adammyhre/Unity-Utils
///// </summary>

namespace Unity6Sample {
  public class Preconditions {
    Preconditions() {
    }

    public static T CheckNotNull<T>(T reference) {
      return CheckNotNull(reference, null);
    }

    public static T CheckNotNull<T>(T reference, string message) {
      if (reference is UnityEngine.Object obj && OrNull(obj) == null) {
        throw new ArgumentNullException(message);
      }

      if (reference is null) {
        throw new ArgumentNullException(message);
      }

      return reference;
    }
    
    public static T OrNull<T>(T obj) where T : UnityEngine.Object {
      return obj == null ? null : obj;
    }

    public static void CheckState(bool expression) {
      CheckState(expression, null);
    }

    public static void CheckState(bool expression, string messageTemplate, params object[] messageArgs) {
      CheckState(expression, string.Format(messageTemplate, messageArgs));
    }

    public static void CheckState(bool expression, string message) {
      if (expression) {
        return;
      }

      throw message == null ? new InvalidOperationException() : new InvalidOperationException(message);
    }
  }
}