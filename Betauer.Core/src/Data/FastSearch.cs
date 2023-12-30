using System;
using System.Collections.Generic;

namespace Betauer.Core.Data;

/// <summary>
/// Find the smallest element in an unsorted array without allocating memory with LINQ or enumerators.
/// It troes to use the fastest method depending on the number of elements in the array.
/// The toNumber
/// </summary>
public static class FastSearch {
    public static T FindMinimumValue<T>(List<T> array, Func<T, int> toNumber) {
        if (array == null) throw new ArgumentException("Parameter error: array can't be null");
        var length = array.Count;
        if (length == 0) throw new ArgumentException("Parameter error: array can't be empty");
        T minValue = array[0];
        if (length == 1) return minValue;
        var minValueNumeric = toNumber(minValue);
        if (length == 2) {
            if (toNumber(array[1]) < minValueNumeric) {
                minValue = array[1];
            }
            return minValue;
        }
        for (var n = 1; n < length; n++) {
            var currentValueNumeric = toNumber(array[n]);
            if (currentValueNumeric < minValueNumeric) {
                minValue = array[n];
                minValueNumeric = currentValueNumeric;
            }
        }
        return minValue;
    }

    public static T FindMinimumValue<T>(List<T> array, Func<T, float> toNumber) {
        if (array == null) throw new ArgumentException("Parameter error: array can't be null");
        var length = array.Count;
        if (length == 0) throw new ArgumentException("Parameter error: array can't be empty");
        T minValue = array[0];
        if (length == 1) return minValue;
        var minValueNumeric = toNumber(minValue);
        if (length == 2) {
            if (toNumber(array[1]) < minValueNumeric) {
                minValue = array[1];
            }
            return minValue;
        }
        for (var n = 1; n < length; n++) {
            var currentValueNumeric = toNumber(array[n]);
            if (currentValueNumeric < minValueNumeric) {
                minValue = array[n];
                minValueNumeric = currentValueNumeric;
            }
        }
        return minValue;
    }

    public static T FindMinimumValue<T>(List<T> array, Comparison<T> comparison) {
        if (array == null) throw new ArgumentException("Parameter error: array can't be null");
        var length = array.Count;
        if (length == 0) throw new ArgumentException("Parameter error: array can't be empty");
        T minValue = array[0];
        if (length == 1) return minValue;
        if (length == 2) {
            if (comparison(array[1], minValue) < 0) {
                minValue = array[1];
            }
            return minValue;
        }
        for (var n = 1; n < length; n++) {
            if (comparison(array[n], minValue) < 0) {
                minValue = array[n];
            }
        }
        return minValue;
    }
}