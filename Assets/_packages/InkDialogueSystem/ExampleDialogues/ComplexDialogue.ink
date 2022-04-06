INCLUDE globals.ink

Hello there! #speaker:Green #portrait:green_neutral
{pokemon_name == "": 
    -> main1
 -else:
    -> main2
}

=== main1 ===
{hasTalked1:
    Come back when you choose a pokemon.
    -> DONE
 -else:
    The day is quite beautiful today.
    I love sunny days! #portrait:green_happy
    Talk again to me when you choose a pokemon. #portrait:green_neutral
    ~ hasTalked1 = true
    -> DONE
}

=== main2 ===
{hasTalked2:
    It was nice to talk to you.
    -> END
 -else:
    I see that you chose {pokemon_name}.
    That was a good choice! #portrait:green_happy
    ~ hasTalked2 = true
    -> DONE
}