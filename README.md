# Science Week "Moving Programs"

In the time of one week we have been studying the schooling behavior of and how we can implement it in code. We have come to the conclusion that **Boids** are the best way of doing what we wanted.

## Boids

**Boids** are a generalized life form, which has some kind of flocking behavior. Using the rules first developed by [Craig Reynolds](<https://en.wikipedia.org/wiki/Craig_Raynolds_(computer_graphics)>) in 1986 we can Simulate this behavior and implement it rather easy in code.

### Rules for Boids

There are three rules that need to be applied to the Boids to achive the wanted flocking behavior.
The first is **Separation** which defines what distance the Boids should have from each other.
The next is **Alignment**, it aligns the Boids so they all go into the same direction.
And the last is **Cohesion** this defines a center to the flock of Boids to which they get drawn to.

