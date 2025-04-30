import {
  animate,
  AnimationTriggerMetadata,
  style,
  transition,
  trigger,
} from '@angular/animations';

export const slideUpAnimation: AnimationTriggerMetadata = trigger('slideUp', [
  transition(':enter', [
    style({ transform: 'translateY(-20px)', opacity: 0 }),
    animate(
      '400ms ease-out',
      style({ transform: 'translateY(0)', opacity: 1 })
    ),
  ]),
]);
