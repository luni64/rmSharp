## TreeGenerator

This is a somewhat larger example showing how to generate a random tree.

The code will create individuals with random names taken from `givenFemales.txt`, `givenMales.txt`, and `givenNames.txt`. It will also create families with a random number of children in them.

Birth, death, and marriage events are given reasonably calculated dates. The places and locations for these events are taken from the `us_cities.txt` file. The example assigns random birthplaces for the individuals and the place of their marriage. However, it takes care that the birthplaces of both spouses and the place of their marriage are located in a 150km circle. Children are born at the marriage place. Thus, the 150km circle can slowly move around through the generations.

The example can generate as many trees in one database as you like. Each one can be configured by the number of generations generated, the location, and the birthday of the starting person.

The number of children and the ages at marriage and death are calculated statistically. Therefore, the number of persons and families generated will vary from run to run.
