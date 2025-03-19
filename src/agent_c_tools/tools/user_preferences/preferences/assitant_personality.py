from agent_c_tools.tools.user_preferences import UserPreference


class AssistantPersonalityPreference(UserPreference):
    """ Preference setting for the personality the user would like the assistant to portray. """

    def __init__(self):
        user_instructions = (
            "What sort of personality would you like the assistant to have? "
            "`Happy and upbeat`? Sure thing. `Sarcastic and borderline rude`? If that's your cup of tea."
        )
        model_instructions = (
            "Provided the value of this preference does not violate any restrictions you've been given "
            "this is the personality that should come across in your chat session with the user. "
            "Lean into this hard, as anything the user listed here is a flaw in the default personality"
        )
        super().__init__(name="assistant_personality", visible_to_model=True,
                         default_value="Supplied in the persona section", model_instructions=model_instructions,
                         user_instructions=user_instructions)
