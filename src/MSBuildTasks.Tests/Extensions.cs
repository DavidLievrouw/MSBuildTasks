using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FakeItEasy;
using KellermanSoftware.CompareNetObjects;

namespace DavidLievrouw.MSBuildTasks {
  public static class Extensions {
    public static T CreateUninitializedInstance<T>() where T : class {
      return (T) FormatterServices.GetUninitializedObject(typeof(T));
    }

    public static T CreateUninitializedInstance<T>(this T reference) where T : class {
      return CreateUninitializedInstance<T>();
    }

    public static bool HasSamePropertyValuesAs<T>(this T first, T second) {
      return HasSamePropertyValuesAs(first, second, false, null);
    }

    public static bool HasSamePropertyValuesAs<T>(this T first, T second, bool ignoreCollectionOrder) {
      return HasSamePropertyValuesAs(first, second, ignoreCollectionOrder, null);
    }

    public static bool HasSamePropertyValuesAs<T>(this T first, T second, IEnumerable<string> membersToIgnore) {
      return HasSamePropertyValuesAs(first, second, false, membersToIgnore);
    }

    public static bool HasSamePropertyValuesAs<T>(this T first, T second, bool ignoreCollectionOrder, IEnumerable<string> membersToIgnore) {
      var compareLogic = new CompareLogic {
        Config = new ComparisonConfig {
          IgnoreObjectTypes = true, // allows anonymous types to be compared
          IgnoreCollectionOrder = ignoreCollectionOrder,
          MembersToIgnore = membersToIgnore == null
            ? new List<string>()
            : membersToIgnore.ToList()
        }
      };

      return compareLogic.Compare(first, second).AreEqual;
    }

    public static bool IsSameCollectionAs<T>(this IEnumerable<T> first, IEnumerable<T> second) {
      return IsSameCollectionAs(first, second, null);
    }

    public static bool IsSameCollectionAs<T>(this IEnumerable<T> first, IEnumerable<T> second, IEnumerable<string> membersToIgnore) {
      if (first == null && second == null) return true;
      if (first == null || second == null) return false;

      var materializedFirst = first.ToList();
      var materializedSecond = second.ToList();
      var compareLogic = new CompareLogic {
        Config = new ComparisonConfig {
          IgnoreObjectTypes = true, // allows anonymous types to be compared
          IgnoreCollectionOrder = true,
          MembersToIgnore = membersToIgnore == null
            ? new List<string>()
            : membersToIgnore.ToList()
        }
      };

      return
        materializedFirst.Count == materializedSecond.Count &&
        materializedFirst.All(itemFromFirst => materializedSecond.Any(itemFromSecond => compareLogic.Compare(itemFromFirst, itemFromSecond).AreEqual));
    }

    public static T Fake<T>(this T reference) {
      return A.Fake<T>();
    }

    public static T Dummy<T>(this T reference) {
      return A.Dummy<T>();
    }

    public static bool NoDependenciesAreOptional<T>(this T reference) {
      var constructors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
      if (constructors.Length < 1) throw new NoPublicConstructorsException();
      if (constructors.Length > 1) throw new MultiplePublicConstructorsException();

      var ctor = constructors.Single();
      var dummyMethod = typeof(A).GetMethod("Dummy");
      var ctorArguments = ctor.GetParameters().Select(p => dummyMethod.MakeGenericMethod(p.ParameterType).Invoke(null, null)).ToArray();

      if (ctorArguments.Length < 1) return true;
      for (var i = 0; i < ctorArguments.Length; i++) {
        try {
          var args = new object[ctorArguments.Length];
          Array.Copy(ctorArguments, args, ctorArguments.Length);
          args[i] = null;
          ctor.Invoke(args);
          return false;
        } catch (TargetInvocationException ex) {
          if (!(ex.InnerException is ArgumentNullException)) {
            // Not an ArgumentNullException
            return false;
          }
        } catch (ArgumentNullException) {
          // Ctor fails with ArgumentNullException when null is passed, as expected
        }
      }
      return true;
    }

    public class MultiplePublicConstructorsException : Exception {}

    public class NoPublicConstructorsException : Exception {}
  }
}