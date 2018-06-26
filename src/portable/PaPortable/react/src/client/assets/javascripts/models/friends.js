type FriendById = {
  id?: number,
  name: string,
  avatar: string
};

// This is the model of our module state
export type State = {
  friends: number[],
  friendsById: Array<FriendById>
};
