
if (!Array.prototype.distinct) {

    Array.prototype.contains = function (needle) {
        for (kljhgf in this) {
            if (this[kljhgf] == needle) return true;
        }
        return false;
    }
    
    
    //Returns a new array with distinct elements    
    // Usage: distinct([1, 2, 2, 3])
    // Result: [1,2,3]
    Array.prototype.distinct = function () {
        var result = [];

        for (var i = 0; i < this.length; i++) {
            var item = this[i];

            if ($.inArray(item, result) === -1) {
                result.push(item);
            }
        }

        return result;
    }

    // Returns the last item in an array.
    Array.prototype.getLast = function () {
        if (this != null)
            if (this.length > 0)
                return this[this.length - 1];

        return null;
    }

    // Sums the items on the array.
    Array.prototype.sum = function () {
        var result = 0, l = this.length;
        if (l) {
            while (l--) {
                if (this[l] != null) result += parseFloat(this[l]);
            }
        }
        return result;
    }

    // Returns the average of the array.
    Array.prototype.average = function () {
        return this.length ? this.sum() / this.length : 0;
    }

    // Create a CSV string from the array.
    Array.prototype.toCSV = function () {
        var text = "";
        if (l) {
            while (l--) {
                if (this[l] != null) text += this[l].toString() + ",";
            }
        }
        return txt.replace(/,(\s+)?$/, '');
    }

}

ifm_Array = new function () {
    /*
    * Checks if all function arguments are arrays
    */ 
    var checkIfAllArgumentsAreArrays = function (functionArguments) {
        for (var i = 0; i < functionArguments.length; i++) {
            if (!(functionArguments[i] instanceof Array)) {
                throw new Error('Every argument must be an array!');
            }
        }
    }
    
    // Return a new array with distinct elements of all arrays 
    // Usage: union([1, 2, 2, 3], [2, 3, 4, 5, 5])
    // Result: [1,2,3,4,5]
    this.union = function (/* minimum 2 arrays */) {
        if (arguments.length < 2) throw new Error('There must be minimum 2 array arguments!');
        checkIfAllArgumentsAreArrays(arguments);

        var result = arguments[0].distinct();

        for (var i = 1; i < arguments.length; i++) {
            var arrayArgument = arguments[i];

            for (var j = 0; j < arrayArgument.length; j++) {
                var item = arrayArgument[j];

                if ($.inArray(item, result) === -1) {
                    result.push(item);
                }
            }
        }

        return result;
    };

    // Returns a new array with distinct elements that exist in every array
    // Usage: intersect([1, 2, 2, 3], [2, 3, 4, 5, 5])
    // Result: [2,3]
    this.intersect = function (/* minimum 2 arrays */) {
        if (arguments.length < 2) throw new Error('There must be minimum 2 array arguments!');
        checkIfAllArgumentsAreArrays(arguments);

        var result = [];
        var distinctArray = arguments[0].distinct();
        if (distinctArray.length === 0) return [];

        for (var i = 0; i < distinctArray.length; i++) {
            var item = distinctArray[i];

            var shouldAddToResult = true;

            for (var j = 1; j < arguments.length; j++) {
                var array2 = arguments[j];
                if (array2.length == 0) return [];

                if ($.inArray(item, array2) === -1) {
                    shouldAddToResult = false;
                    break;
                }
            }

            if (shouldAddToResult) {
                result.push(item);
            }
        }

        return result;
    };

    // Returns a new array with distinct elements from the first array input elements which are not present in the other arrays
    // Usage: except([1, 2, 2, 3], [2, 3, 4, 5, 5])
    // Result: [1]
    this.except = function (/* minimum 2 arrays */) {
        if (arguments.length < 2) throw new Error('There must be minimum 2 array arguments!');
        checkIfAllArgumentsAreArrays(arguments);

        var result = [];
        var distinctArray = arguments[0].distinct();
        var otherArraysConcatenated = [];

        for (var i = 1; i < arguments.length; i++) {
            var otherArray = arguments[i];
            otherArraysConcatenated = otherArraysConcatenated.concat(otherArray);
        }

        for (var i = 0; i < distinctArray.length; i++) {
            var item = distinctArray[i];

            if ($.inArray(item, otherArraysConcatenated) === -1) {
                result.push(item);
            }
        }

        return result;
    };


}; // Array_IFM END

