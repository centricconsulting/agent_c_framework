from agent_c_tools.tools.user_preferences import UserPreference


class AddressMeAsPreference(UserPreference):
    """ Preference setting for how the user would like to be addressed by the assistant. """

    def __init__(self):
        user_instructions = (
            "How would you like the assistant to address you in your chat session? "
            "'First name'? 'Doctor Last Name'? 'Grand Poobah'? Your choice."
        )
        model_instructions = (
            "Provided the value of this preference does not violate any restrictions you've been given "
            "this is how you should should address the user when chatting."
        )
        super().__init__(name="address_me_as", visible_to_model=True, default_value="First name",
                         model_instructions=model_instructions, user_instructions=user_instructions)
