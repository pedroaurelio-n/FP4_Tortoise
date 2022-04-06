INCLUDE globals.ink

{ pokemon_name == "": -> main | -> already_chose }

===main===
Which pokemon do you choose?
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Charmander]
        -> chosen("Charmander")
    + [Squirtle]
        -> chosen("Squirtle")
    
===chosen(pokemon)===
~ pokemon_name = pokemon
You chose {pokemon}!
-> END

===already_chose===
You already chose {pokemon_name}!
-> END