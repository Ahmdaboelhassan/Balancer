export enum PaidMethods {
  Once = 1,
  Incrementals = 2,
}

export const PaidMethodOptions = [
  { value: PaidMethods.Once, label: 'Once' },
  { value: PaidMethods.Incrementals, label: 'Incrementals' },
];
