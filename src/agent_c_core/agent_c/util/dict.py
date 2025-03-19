def filter_dict_by_keys(source_dict, keys_to_include):
    """
    Create a new dictionary containing only the specified keys.

    :param source_dict: The original dictionary to filter.
    :param keys_to_include: An iterable of keys to include in the new dictionary.
    :return: A new dictionary with only the key-value pairs where the key is in keys_to_include.
    """
    return {key: source_dict[key] for key in keys_to_include if key in source_dict}