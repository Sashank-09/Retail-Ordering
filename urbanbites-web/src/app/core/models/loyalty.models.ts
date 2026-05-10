export interface LoyaltyTransaction {
  points: number;
  reason: string;
  type: 'Earned' | 'Redeemed';
  earnedAt: string;
}

export interface LoyaltyBalance {
  totalPoints: number;
  equivalentAmount: number;
  transactions: LoyaltyTransaction[];
}
