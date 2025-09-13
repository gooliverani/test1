import api from './api';

export interface Profile {
  id: string;
  externalId?: string;
  type: string;
  status: string;
  displayName: string;
  email?: string;
  department?: string;
  createdAt: string;
  modifiedAt?: string;
}

export interface CreateProfileRequest {
  externalId?: string;
  type: string;
  displayName: string;
  email?: string;
  department?: string;
}

export async function listProfiles(): Promise<Profile[]> {
  const res = await api.get<Profile[]>('/profiles');
  return res.data;
}

export async function getProfile(id: string): Promise<Profile> {
  const res = await api.get<Profile>(`/profiles/${id}`);
  return res.data;
}

export async function createProfile(payload: CreateProfileRequest): Promise<Profile> {
  const res = await api.post<Profile>('/profiles', payload);
  return res.data;
}

export async function updateProfileStatus(id: string, status: string): Promise<Profile> {
  const res = await api.patch<Profile>(`/profiles/${id}`, { status });
  return res.data;
}
