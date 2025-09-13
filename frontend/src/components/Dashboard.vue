<template>
  <div class="dashboard" :class="{ 'dark-theme': isDarkMode }">
    <!-- Title Bar -->
    <div class="title-bar">
      <div class="title-bar-content">
        <div class="app-info">
          <div class="app-logo">
            <div class="logo-circle">
              <span class="logo-text">gooaccess</span>
            </div>
          </div>
          <span class="app-name">Employee Access Control</span>
        </div>
        <div class="window-controls">
          <button @click="toggleTheme" class="theme-toggle" :title="isDarkMode ? 'Switch to Light Mode' : 'Switch to Dark Mode'">
            <svg v-if="isDarkMode" width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 7c-2.76 0-5 2.24-5 5s2.24 5 5 5 5-2.24 5-5-2.24-5-5-5zM2 13h2c.55 0 1-.45 1-1s-.45-1-1-1H2c-.55 0-1 .45-1 1s.45 1 1 1zm18 0h2c.55 0 1-.45 1-1s-.45-1-1-1h-2c-.55 0-1 .45-1 1s.45 1 1 1zM11 2v2c0 .55.45 1 1 1s1-.45 1-1V2c0-.55-.45-1-1-1s-1 .45-1 1zm0 18v2c0 .55.45 1 1 1s1-.45 1-1v-2c0-.55-.45-1-1-1s-1 .45-1 1zM5.99 4.58c-.39-.39-1.03-.39-1.41 0-.39.39-.39 1.03 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0s.39-1.03 0-1.41L5.99 4.58zm12.37 12.37c-.39-.39-1.03-.39-1.41 0-.39.39-.39 1.03 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0 .39-.39.39-1.03 0-1.41l-1.06-1.06zm1.06-10.96c.39-.39.39-1.03 0-1.41-.39-.39-1.03-.39-1.41 0l-1.06 1.06c-.39.39-.39 1.03 0 1.41s1.03.39 1.41 0l1.06-1.06zM7.05 18.36c.39-.39.39-1.03 0-1.41-.39-.39-1.03-.39-1.41 0l-1.06 1.06c-.39.39-.39 1.03 0 1.41s1.03.39 1.41 0l1.06-1.06z"/>
            </svg>
            <svg v-else width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12.34 2.02C6.59 1.82 2 6.42 2 12c0 5.52 4.48 10 10 10 3.71 0 6.93-2.02 8.66-5.02-7.51-.25-13.66-6.4-13.66-13.96 0-.67.05-1.35.14-2z"/>
            </svg>
          </button>
          <button class="control-btn minimize">−</button>
          <button class="control-btn maximize">□</button>
          <button class="control-btn close">×</button>
        </div>
      </div>
    </div>

    <!-- Header Section -->
    <div class="header-section">
      <div class="header-content">
        <div class="stats-section">
          <div class="stat-card">
            <div class="stat-icon">👥</div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.totalEmployees }}</div>
              <div class="stat-label">Total Employees</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">✅</div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.activeEmployees }}</div>
              <div class="stat-label">Active</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">🏢</div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.departments }}</div>
              <div class="stat-label">Departments</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-content">
        <div class="search-section">
          <div class="search-group">
            <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
              <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
            </svg>
            <input 
              v-model="searchTerm" 
              type="text" 
              placeholder="Search employees..."
              class="search-input"
            />
          </div>
          <select v-model="selectedDepartment" class="department-filter">
            <option value="">All Departments</option>
            <option v-for="dept in departments" :key="dept" :value="dept">
              {{ dept }}
            </option>
          </select>
          <button @click="refreshData" class="refresh-btn">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 3a5 5 0 1 0 4.546 2.914.5.5 0 0 1 .908-.417A6 6 0 1 1 8 2v1z"/>
              <path d="M8 4.466V.534a.25.25 0 0 1 .41-.192l2.36 1.966c.12.1.12.284 0 .384L8.41 4.658A.25.25 0 0 1 8 4.466z"/>
            </svg>
            Refresh
          </button>
          <button @click="openNewEmployeeModal" class="new-profile-btn">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
              <path d="M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4z"/>
            </svg>
            New Profile
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <div class="content-container">
        <!-- Loading State -->
        <div v-if="loading" class="loading-section">
          <div class="loading-spinner"></div>
          <p>Loading employees...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="error-section">
          <div class="error-icon">⚠️</div>
          <h3>Connection Error</h3>
          <p>{{ error }}</p>
          <button @click="refreshData" class="retry-btn">Try Again</button>
        </div>

        <!-- Main Layout with Sidebar -->
        <div v-else class="main-layout">
          <!-- Employee List Sidebar -->
          <div class="employee-sidebar">
            <div class="sidebar-header">
              <h3>Employees ({{ filteredEmployees.length }})</h3>
            </div>
            
            <div class="employee-list-sidebar">
              <div 
                v-for="employee in filteredEmployees" 
                :key="employee.id"
                class="employee-item"
                :class="{ 
                  inactive: !employee.isActive,
                  selected: selectedEmployee?.id === employee.id 
                }"
                @click="selectEmployee(employee)"
              >
                <div class="employee-item-content">
                  <div class="employee-name">
                    {{ employee.firstName }} {{ employee.lastName }}
                  </div>
                  <div class="employee-details">
                    <span class="employee-id">{{ employee.compId }}</span>
                    <span class="employee-department">{{ employee.department }}</span>
                  </div>
                  <div class="status-indicator" :class="employee.isActive ? 'active' : 'inactive'"></div>
                </div>
              </div>
            </div>
          </div>

          <!-- Employee Details Panel -->
          <div class="employee-details-panel">
            <div v-if="selectedEmployee" class="employee-profile">
              <!-- Profile Header -->
              <div class="profile-header">
                <div class="profile-photo-section">
                  <div class="employee-photo-large">
                    <img 
                      v-if="selectedEmployee.photoUrl" 
                      :src="selectedEmployee.photoUrl" 
                      :alt="`${selectedEmployee.firstName} ${selectedEmployee.lastName}`"
                      @error="handleImageError"
                    />
                    <div v-else class="photo-placeholder-large">
                      {{ selectedEmployee.firstName.charAt(0) }}{{ selectedEmployee.lastName.charAt(0) }}
                    </div>
                  </div>
                </div>
                
                <div class="profile-info">
                  <h2 class="employee-full-name">
                    {{ selectedEmployee.firstName }} {{ selectedEmployee.lastName }}
                  </h2>
                  <div class="employee-meta">
                    <div class="meta-item">
                      <strong>ID:</strong> {{ selectedEmployee.compId }}
                    </div>
                    <div class="meta-item">
                      <strong>Email:</strong> {{ selectedEmployee.email }}
                    </div>
                    <div class="meta-item">
                      <strong>Department:</strong> {{ selectedEmployee.department }}
                    </div>
                    <div class="meta-item">
                      <strong>Location:</strong> {{ selectedEmployee.location }}
                    </div>
                    <div class="meta-item">
                      <strong>Badge:</strong> {{ selectedEmployee.badgeSerial }}
                    </div>
                    <div class="meta-item">
                      <strong>Status:</strong> 
                      <span class="status-badge" :class="selectedEmployee.isActive ? 'active' : 'inactive'">
                        {{ selectedEmployee.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Tabbed Content -->
              <div class="profile-tabs">
                <div class="tab-headers">
                  <button 
                    v-for="tab in tabs" 
                    :key="tab.id"
                    class="tab-header"
                    :class="{ active: activeTab === tab.id }"
                    @click="activeTab = tab.id"
                  >
                    <span class="tab-icon">{{ tab.icon }}</span>
                    {{ tab.label }}
                  </button>
                </div>

                <div class="tab-content">
                  <!-- Employee Details Tab -->
                  <div v-if="activeTab === 'details'" class="tab-panel">
                    <div class="details-grid">
                      <div class="detail-group">
                        <h4>Employment Information</h4>
                        <div class="detail-item">
                          <strong>Hire Date:</strong> {{ formatDate(selectedEmployee.hireDate) }}
                        </div>
                        <div class="detail-item" v-if="selectedEmployee.expireDate">
                          <strong>Expire Date:</strong> {{ formatDate(selectedEmployee.expireDate) }}
                        </div>
                        <div class="detail-item" v-if="selectedEmployee.team">
                          <strong>Team:</strong> {{ selectedEmployee.team }}
                        </div>
                      </div>
                      
                      <div class="detail-group">
                        <h4>Access Profiles</h4>
                        <div class="access-profiles-grid">
                          <span 
                            v-for="profile in selectedEmployee.accessProfiles" 
                            :key="profile"
                            class="profile-tag"
                          >
                            {{ profile }}
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Badge History Tab -->
                  <div v-if="activeTab === 'badge'" class="tab-panel">
                    <div v-if="loadingBadgeHistory" class="loading-section">
                      <div class="loading-spinner"></div>
                      <p>Loading badge history...</p>
                    </div>
                    <div v-else-if="badgeHistory.length === 0" class="empty-state">
                      <p>No badge history found</p>
                    </div>
                    <div v-else class="history-list">
                      <div 
                        v-for="entry in badgeHistory" 
                        :key="entry.id"
                        class="history-item"
                      >
                        <div class="history-header">
                          <span class="action-type">{{ entry.actionType }}</span>
                          <span class="timestamp">{{ formatDate(entry.issuedDate) }}</span>
                        </div>
                        <div class="history-details">
                          <div><strong>Badge Serial:</strong> {{ entry.badgeSerial }}</div>
                          <div v-if="entry.previousBadgeSerial">
                            <strong>Previous Badge:</strong> {{ entry.previousBadgeSerial }}
                          </div>
                          <div v-if="entry.issuedBy">
                            <strong>Issued By:</strong> {{ entry.issuedBy }}
                          </div>
                          <div v-if="entry.reason">
                            <strong>Reason:</strong> {{ entry.reason }}
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Access History Tab -->
                  <div v-if="activeTab === 'access'" class="tab-panel">
                    <div v-if="loadingAccessHistory" class="loading-section">
                      <div class="loading-spinner"></div>
                      <p>Loading access history...</p>
                    </div>
                    <div v-else-if="accessHistory.length === 0" class="empty-state">
                      <p>No access history found</p>
                    </div>
                    <div v-else class="history-list">
                      <div 
                        v-for="entry in accessHistory" 
                        :key="entry.id"
                        class="history-item"
                      >
                        <div class="history-header">
                          <span class="action-type">{{ entry.actionType }}</span>
                          <span class="timestamp">{{ formatDate(entry.effectiveDate) }}</span>
                        </div>
                        <div class="history-details">
                          <div v-if="entry.accessProfile">
                            <strong>Access Profile:</strong> {{ entry.accessProfile.name }}
                          </div>
                          <div v-if="entry.previousAccessProfile">
                            <strong>Previous Profile:</strong> {{ entry.previousAccessProfile.name }}
                          </div>
                          <div v-if="entry.grantedBy">
                            <strong>Granted By:</strong> {{ entry.grantedBy }}
                          </div>
                          <div v-if="entry.reason">
                            <strong>Reason:</strong> {{ entry.reason }}
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Swipe Logs Tab -->
                  <div v-if="activeTab === 'swipes'" class="tab-panel">
                    <div v-if="loadingSwipes" class="loading-section">
                      <div class="loading-spinner"></div>
                      <p>Loading swipe logs...</p>
                    </div>
                    <div v-else-if="swipes.length === 0" class="empty-state">
                      <p>No recent swipes found</p>
                    </div>
                    <div v-else class="history-list">
                      <div 
                        v-for="swipe in swipes" 
                        :key="swipe.id"
                        class="history-item"
                        :class="{ denied: !swipe.accessGranted }"
                      >
                        <div class="history-header">
                          <span class="action-type" :class="swipe.accessGranted ? 'granted' : 'denied'">
                            {{ swipe.accessGranted ? 'Access Granted' : 'Access Denied' }}
                          </span>
                          <span class="timestamp">{{ formatDate(swipe.swipeTime) }}</span>
                        </div>
                        <div class="history-details">
                          <div><strong>Card Number:</strong> {{ swipe.cardNumber }}</div>
                          <div><strong>Reader ID:</strong> {{ swipe.readerId }}</div>
                          <div v-if="swipe.denialReason">
                            <strong>Denial Reason:</strong> {{ swipe.denialReason }}
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Card Fields Tab -->
                  <div v-if="activeTab === 'cardfields'" class="tab-panel">
                    <div v-if="loadingCardFields" class="loading-section">
                      <div class="loading-spinner"></div>
                      <p>Loading card fields...</p>
                    </div>
                    <div v-else class="card-fields-content">
                      <!-- Summary Cards -->
                      <div v-if="cardFieldsSummary" class="summary-cards">
                        <div class="summary-card">
                          <div class="summary-icon">🎛️</div>
                          <div class="summary-info">
                            <div class="summary-number">{{ cardFieldsSummary.totalReaders }}</div>
                            <div class="summary-label">Total Readers</div>
                          </div>
                        </div>
                        <div class="summary-card">
                          <div class="summary-icon">🔑</div>
                          <div class="summary-info">
                            <div class="summary-number">{{ cardFieldsSummary.totalAccessProfiles }}</div>
                            <div class="summary-label">Access Profiles</div>
                          </div>
                        </div>
                        <div class="summary-card">
                          <div class="summary-icon">🔗</div>
                          <div class="summary-info">
                            <div class="summary-number">{{ cardFieldsSummary.totalAssignments }}</div>
                            <div class="summary-label">Profile-Reader Links</div>
                          </div>
                        </div>
                        <div class="summary-card">
                          <div class="summary-icon">⚠️</div>
                          <div class="summary-info">
                            <div class="summary-number">{{ cardFieldsSummary.readersWithoutProfiles }}</div>
                            <div class="summary-label">Unassigned Readers</div>
                          </div>
                        </div>
                      </div>

                      <!-- Card Fields Sections -->
                      <div class="card-fields-sections">
                        <!-- Readers Section -->
                        <div class="field-section">
                          <h4>Card Readers ({{ readers.length }})</h4>
                          <div class="cards-grid">
                            <div 
                              v-for="reader in readers" 
                              :key="reader.id"
                              class="field-card reader-card"
                            >
                              <div class="card-header">
                                <div class="card-icon">🎛️</div>
                                <div class="card-title">{{ reader.name }}</div>
                              </div>
                              <div class="card-details">
                                <div v-if="reader.location" class="detail-item">
                                  <strong>Location:</strong> {{ reader.location }}
                                </div>
                                <div v-if="reader.deviceType" class="detail-item">
                                  <strong>Type:</strong> {{ reader.deviceType }}
                                </div>
                                <div class="detail-item">
                                  <strong>Access Profiles:</strong> {{ reader.accessProfileCount }}
                                </div>
                              </div>
                              <div v-if="reader.accessProfiles.length > 0" class="card-profiles">
                                <div class="profiles-list">
                                  <span 
                                    v-for="profile in reader.accessProfiles.slice(0, 3)" 
                                    :key="profile.id"
                                    class="profile-tag"
                                  >
                                    {{ profile.name }}
                                  </span>
                                  <span v-if="reader.accessProfiles.length > 3" class="more-indicator">
                                    +{{ reader.accessProfiles.length - 3 }} more
                                  </span>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>

                        <!-- Access Profiles Section -->
                        <div class="field-section">
                          <h4>Access Profiles ({{ accessProfiles.length }})</h4>
                          <div class="cards-grid">
                            <div 
                              v-for="profile in accessProfiles" 
                              :key="profile.id"
                              class="field-card profile-card"
                              :class="{ inactive: !profile.isActive }"
                            >
                              <div class="card-header">
                                <div class="card-icon">🔑</div>
                                <div class="card-title">{{ profile.name }}</div>
                                <div class="status-indicator" :class="profile.isActive ? 'active' : 'inactive'"></div>
                              </div>
                              <div class="card-details">
                                <div v-if="profile.description" class="detail-item">
                                  <strong>Description:</strong> {{ profile.description }}
                                </div>
                                <div class="detail-item">
                                  <strong>Employees:</strong> {{ profile.employeeCount }}
                                </div>
                                <div class="detail-item">
                                  <strong>Readers:</strong> {{ profile.readerCount }}
                                </div>
                                <div class="detail-item">
                                  <strong>Created:</strong> {{ formatDate(profile.createdAt) }}
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- No Selection State -->
            <div v-else class="no-selection">
              <div class="no-selection-content">
                <div class="no-selection-icon">👤</div>
                <h3>Select an Employee</h3>
                <p>Choose an employee from the list to view their detailed profile, badge history, and access logs.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Employee Modal -->
    <div v-if="showNewEmployeeModal" class="modal-overlay" @click="closeNewEmployeeModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Create New Employee Profile</h2>
          <button @click="closeNewEmployeeModal" class="modal-close">×</button>
        </div>
        
        <div class="modal-body">
          <form @submit.prevent="saveNewEmployee" class="employee-form">
            <div class="form-grid">
              <!-- Personal Information -->
              <div class="form-section">
                <h3>Personal Information</h3>
                <div class="form-row">
                  <div class="form-group">
                    <label for="firstName">First Name *</label>
                    <input 
                      id="firstName"
                      v-model="newEmployeeForm.firstName" 
                      type="text" 
                      required 
                      class="form-input"
                      placeholder="Enter first name"
                    />
                  </div>
                  <div class="form-group">
                    <label for="lastName">Last Name *</label>
                    <input 
                      id="lastName"
                      v-model="newEmployeeForm.lastName" 
                      type="text" 
                      required 
                      class="form-input"
                      placeholder="Enter last name"
                    />
                  </div>
                </div>
                
                <div class="form-group">
                  <label for="email">Email Address *</label>
                  <input 
                    id="email"
                    v-model="newEmployeeForm.email" 
                    type="email" 
                    required 
                    class="form-input"
                    placeholder="Enter email address"
                  />
                </div>
              </div>

              <!-- Employment Details -->
              <div class="form-section">
                <h3>Employment Details</h3>
                <div class="form-row">
                  <div class="form-group">
                    <label for="departmentId">Department</label>
                    <select 
                      id="departmentId"
                      v-model="newEmployeeForm.departmentId" 
                      class="form-select"
                    >
                      <option value="">Select Department</option>
                      <option v-for="dept in departments" :key="dept" :value="dept">
                        {{ dept }}
                      </option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label for="locationId">Location</label>
                    <select 
                      id="locationId"
                      v-model="newEmployeeForm.locationId" 
                      class="form-select"
                    >
                      <option value="">Select Location</option>
                      <!-- Add location options here -->
                    </select>
                  </div>
                </div>
                
                <div class="form-row">
                  <div class="form-group">
                    <label for="hireDate">Hire Date</label>
                    <input 
                      id="hireDate"
                      v-model="newEmployeeForm.hireDate" 
                      type="date" 
                      class="form-input"
                    />
                  </div>
                  <div class="form-group">
                    <label for="expireDate">Expire Date</label>
                    <input 
                      id="expireDate"
                      v-model="newEmployeeForm.expireDate" 
                      type="date" 
                      class="form-input"
                    />
                  </div>
                </div>
              </div>

              <!-- Badge Information -->
              <div class="form-section">
                <h3>Badge Information</h3>
                <div class="form-row">
                  <div class="form-group">
                    <label for="badgeSerial">Badge Serial</label>
                    <input 
                      id="badgeSerial"
                      v-model="newEmployeeForm.badgeSerial" 
                      type="text" 
                      class="form-input"
                      placeholder="Enter badge serial number"
                    />
                  </div>
                  <div class="form-group checkbox-group">
                    <label class="checkbox-label">
                      <input 
                        v-model="newEmployeeForm.isActive" 
                        type="checkbox" 
                        class="form-checkbox"
                      />
                      <span class="checkbox-text">Active Employee</span>
                    </label>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>
        
        <div class="modal-footer">
          <button @click="closeNewEmployeeModal" type="button" class="btn-cancel">
            Cancel
          </button>
          <button 
            @click="saveNewEmployee" 
            type="button" 
            class="btn-save" 
            :disabled="savingNewEmployee"
          >
            <span v-if="savingNewEmployee">Saving...</span>
            <span v-else>Save Employee</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

interface Employee {
  id: number
  compId: string
  firstName: string
  lastName: string
  email: string
  hireDate: string
  expireDate?: string
  isActive: boolean
  badgeSerial: string
  department: string
  team?: string
  location: string
  accessProfiles: string[]
  photoUrl?: string
}

interface BadgeHistoryEntry {
  id: number
  badgeSerial: string
  actionType: string
  previousBadgeSerial?: string
  issuedDate: string
  expiryDate?: string
  issuedBy?: string
  reason?: string
  isActive: boolean
}

interface AccessHistoryEntry {
  id: number
  actionType: string
  effectiveDate: string
  expiryDate?: string
  grantedBy?: string
  reason?: string
  accessProfile?: { id: number; name: string }
  previousAccessProfile?: { id: number; name: string }
}

interface SwipeEntry {
  id: number
  swipeTime: string
  cardNumber: string
  accessGranted: boolean
  denialReason?: string
  readerId: number
}

interface Tab {
  id: string
  label: string
  icon: string
}

interface Reader {
  id: number
  name: string
  deviceType?: string
  capabilities?: string
  location?: string
  accessProfileCount: number
  accessProfiles: {
    id: number
    name: string
    description?: string
  }[]
}

interface AccessProfile {
  id: number
  name: string
  description?: string
  isActive: boolean
  createdAt: string
  employeeCount: number
  readerCount: number
}

interface AccessProfileReader {
  accessProfileId: number
  accessProfileName: string
  readerId: number
  readerName: string
  readerLocation?: string
  readerDeviceType?: string
}

interface CardFieldsSummary {
  totalReaders: number
  totalAccessProfiles: number
  totalAssignments: number
  activeProfiles: number
  readersWithoutProfiles: number
  profilesWithoutReaders: number
}

const employees = ref<Employee[]>([])
const selectedEmployee = ref<Employee | null>(null)
const loading = ref(true)
const error = ref('')
const searchTerm = ref('')
const selectedDepartment = ref('')
const isDarkMode = ref(false)

// Tab management
const activeTab = ref('details')
const tabs: Tab[] = [
  { id: 'details', label: 'Employee Details', icon: '👤' },
  { id: 'badge', label: 'Badge History', icon: '🎫' },
  { id: 'access', label: 'Access History', icon: '🔑' },
  { id: 'swipes', label: 'Swipe Logs', icon: '📋' },
  { id: 'cardfields', label: 'Card Fields', icon: '🎛️' }
]

// History data
const badgeHistory = ref<BadgeHistoryEntry[]>([])
const accessHistory = ref<AccessHistoryEntry[]>([])
const swipes = ref<SwipeEntry[]>([])

// Loading states for tabs
const loadingBadgeHistory = ref(false)
const loadingAccessHistory = ref(false)
const loadingSwipes = ref(false)
const loadingCardFields = ref(false)

// Card fields data
const readers = ref<Reader[]>([])
const accessProfiles = ref<AccessProfile[]>([])
const accessProfileReaders = ref<AccessProfileReader[]>([])
const cardFieldsSummary = ref<CardFieldsSummary | null>(null)

// New employee modal state
const showNewEmployeeModal = ref(false)
const savingNewEmployee = ref(false)
const newEmployeeForm = ref({
  firstName: '',
  lastName: '',
  email: '',
  departmentId: null as number | null,
  teamId: null as number | null,
  locationId: null as number | null,
  hireDate: new Date().toISOString().split('T')[0],
  expireDate: '',
  badgeSerial: '',
  isActive: true
})

// Check for saved theme preference or default to light mode
onMounted(() => {
  const savedTheme = localStorage.getItem('theme')
  if (savedTheme) {
    isDarkMode.value = savedTheme === 'dark'
  } else if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    isDarkMode.value = true
  }
  
  // Apply theme to body
  document.body.classList.toggle('dark-theme', isDarkMode.value)
})

const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value
  localStorage.setItem('theme', isDarkMode.value ? 'dark' : 'light')
  document.body.classList.toggle('dark-theme', isDarkMode.value)
}

const handleImageError = (event: Event) => {
  const target = event.target as HTMLImageElement
  target.style.display = 'none'
}

const stats = computed(() => ({
  totalEmployees: employees.value.length,
  activeEmployees: employees.value.filter(emp => emp.isActive).length,
  departments: new Set(employees.value.map(emp => emp.department)).size
}))

const departments = computed(() => {
  return Array.from(new Set(employees.value.map(emp => emp.department))).sort()
})

const filteredEmployees = computed(() => {
  let filtered = employees.value

  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase()
    filtered = filtered.filter(emp => 
      emp.firstName.toLowerCase().includes(term) ||
      emp.lastName.toLowerCase().includes(term) ||
      emp.email.toLowerCase().includes(term) ||
      emp.compId.toLowerCase().includes(term)
    )
  }

  if (selectedDepartment.value) {
    filtered = filtered.filter(emp => emp.department === selectedDepartment.value)
  }

  return filtered
})

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

// Employee selection and data fetching
const selectEmployee = async (employee: Employee) => {
  selectedEmployee.value = employee
  activeTab.value = 'details'
  
  // Fetch additional data for the selected employee
  await Promise.all([
    fetchBadgeHistory(employee.id),
    fetchAccessHistory(employee.id),
    fetchSwipes(employee.id)
  ])
}

// Watch for tab changes to load card fields data when needed
watch(activeTab, (newTab) => {
  if (newTab === 'cardfields' && readers.value.length === 0) {
    fetchCardFields()
  }
})

const fetchBadgeHistory = async (employeeId: number) => {
  try {
    loadingBadgeHistory.value = true
    const response = await fetch(`http://localhost:5000/api/employees/${employeeId}/badge-history`)
    if (response.ok) {
      badgeHistory.value = await response.json()
    } else {
      badgeHistory.value = []
    }
  } catch (err) {
    console.error('Error fetching badge history:', err)
    badgeHistory.value = []
  } finally {
    loadingBadgeHistory.value = false
  }
}

const fetchAccessHistory = async (employeeId: number) => {
  try {
    loadingAccessHistory.value = true
    const response = await fetch(`http://localhost:5000/api/employees/${employeeId}/access-history`)
    if (response.ok) {
      accessHistory.value = await response.json()
    } else {
      accessHistory.value = []
    }
  } catch (err) {
    console.error('Error fetching access history:', err)
    accessHistory.value = []
  } finally {
    loadingAccessHistory.value = false
  }
}

const fetchSwipes = async (employeeId: number) => {
  try {
    loadingSwipes.value = true
    const response = await fetch(`http://localhost:5000/api/employees/${employeeId}/swipes`)
    if (response.ok) {
      swipes.value = await response.json()
    } else {
      swipes.value = []
    }
  } catch (err) {
    console.error('Error fetching swipes:', err)
    swipes.value = []
  } finally {
    loadingSwipes.value = false
  }
}

const fetchEmployees = async () => {
  try {
    loading.value = true
    error.value = ''
    
    const response = await fetch('http://localhost:5000/api/employees')
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`)
    }
    
    const data = await response.json()
    // Use photoUrl from the API response
    employees.value = data.map((emp: Employee) => ({
      ...emp
    }))
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to fetch employees'
    console.error('Error fetching employees:', err)
  } finally {
    loading.value = false
  }
}

const fetchCardFields = async () => {
  if (loadingCardFields.value) return
  
  loadingCardFields.value = true
  try {
    // Fetch all card fields data in parallel
    const [readersResponse, profilesResponse, summaryResponse] = await Promise.all([
      fetch('http://localhost:5000/api/employees/readers'),
      fetch('http://localhost:5000/api/employees/access-profiles'),
      fetch('http://localhost:5000/api/employees/card-fields-summary')
    ])

    if (readersResponse.ok) {
      readers.value = await readersResponse.json()
    }
    
    if (profilesResponse.ok) {
      accessProfiles.value = await profilesResponse.json()
    }
    
    if (summaryResponse.ok) {
      cardFieldsSummary.value = await summaryResponse.json()
    }
  } catch (err) {
    console.error('Error fetching card fields:', err)
  } finally {
    loadingCardFields.value = false
  }
}

const refreshData = () => {
  fetchEmployees()
}

// New employee modal functions
const openNewEmployeeModal = () => {
  resetNewEmployeeForm()
  showNewEmployeeModal.value = true
}

const closeNewEmployeeModal = () => {
  showNewEmployeeModal.value = false
  resetNewEmployeeForm()
}

const resetNewEmployeeForm = () => {
  newEmployeeForm.value = {
    firstName: '',
    lastName: '',
    email: '',
    departmentId: null,
    teamId: null,
    locationId: null,
    hireDate: new Date().toISOString().split('T')[0],
    expireDate: '',
    badgeSerial: '',
    isActive: true
  }
}

const saveNewEmployee = async () => {
  if (savingNewEmployee.value) return
  
  // Basic validation
  if (!newEmployeeForm.value.firstName || !newEmployeeForm.value.lastName || !newEmployeeForm.value.email) {
    alert('Please fill in all required fields (First Name, Last Name, Email)')
    return
  }
  
  savingNewEmployee.value = true
  try {
    const response = await fetch('http://localhost:5000/api/employees', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newEmployeeForm.value)
    })
    
    if (response.ok) {
      const newEmployee = await response.json()
      closeNewEmployeeModal()
      // Refresh the employee list and select the new employee
      await fetchEmployees()
      const createdEmployee = employees.value.find(emp => emp.id === newEmployee.id)
      if (createdEmployee) {
        selectEmployee(createdEmployee)
      }
    } else {
      const errorData = await response.json()
      alert(`Error creating employee: ${errorData.message || 'Unknown error'}`)
    }
  } catch (err) {
    console.error('Error creating employee:', err)
    alert('Error creating employee. Please try again.')
  } finally {
    savingNewEmployee.value = false
  }
}

onMounted(() => {
  fetchEmployees()
})
</script>

<style scoped>
/* Windows-style Dark/Light Theme Variables */
.dashboard {
  min-height: 100vh;
  background: #f3f3f3;
  color: #000;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  transition: all 0.3s ease;
}

.dashboard.dark-theme {
  background: #1e1e1e;
  color: #fff;
}

/* Title Bar */
.title-bar {
  background: linear-gradient(to bottom, #f0f0f0, #e5e5e5);
  border-bottom: 1px solid #ccc;
  padding: 8px 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  min-height: 32px;
}

.dark-theme .title-bar {
  background: linear-gradient(to bottom, #3c3c3c, #2d2d2d);
  border-bottom: 1px solid #555;
}

.title-bar-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.app-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.app-logo {
  display: flex;
  align-items: center;
}

.logo-circle {
  width: 24px;
  height: 24px;
  background: linear-gradient(135deg, #4FC08D 0%, #44A08D 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.logo-text {
  font-size: 8px;
  font-weight: bold;
  color: white;
  text-shadow: 0 1px 1px rgba(0, 0, 0, 0.3);
  letter-spacing: -0.5px;
}

.app-name {
  font-size: 13px;
  font-weight: 400;
  color: #333;
}

.dark-theme .app-name {
  color: #fff;
}

.window-controls {
  display: flex;
  align-items: center;
  gap: 4px;
}

.theme-toggle {
  background: none;
  border: 1px solid #ccc;
  border-radius: 3px;
  padding: 4px 8px;
  cursor: pointer;
  color: #666;
  transition: all 0.2s ease;
  margin-right: 8px;
}

.theme-toggle:hover {
  background: #e6e6e6;
  border-color: #999;
}

.dark-theme .theme-toggle {
  border-color: #555;
  color: #ccc;
}

.dark-theme .theme-toggle:hover {
  background: #4a4a4a;
  border-color: #777;
}

.control-btn {
  background: none;
  border: none;
  width: 24px;
  height: 24px;
  cursor: pointer;
  font-size: 14px;
  color: #666;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.control-btn:hover {
  background: #e6e6e6;
}

.control-btn.close:hover {
  background: #e81123;
  color: white;
}

.dark-theme .control-btn {
  color: #ccc;
}

.dark-theme .control-btn:hover {
  background: #4a4a4a;
}

/* Header Section */
.header-section {
  background: linear-gradient(to bottom, #ffffff, #f8f8f8);
  border-bottom: 1px solid #ddd;
  padding: 16px;
}

.dark-theme .header-section {
  background: linear-gradient(to bottom, #2d2d2d, #252525);
  border-bottom: 1px solid #444;
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
}

.stats-section {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}

.stat-card {
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 12px 16px;
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 140px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.dark-theme .stat-card {
  background: #3c3c3c;
  border-color: #555;
  color: #fff;
}

.stat-icon {
  font-size: 20px;
}

.stat-number {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 2px;
}

.stat-label {
  font-size: 12px;
  color: #666;
}

.dark-theme .stat-label {
  color: #ccc;
}

/* Toolbar */
.toolbar {
  background: #f8f8f8;
  border-bottom: 1px solid #ddd;
  padding: 12px 16px;
}

.dark-theme .toolbar {
  background: #252525;
  border-bottom: 1px solid #444;
}

.toolbar-content {
  max-width: 1400px;
  margin: 0 auto;
}

.search-section {
  display: flex;
  gap: 12px;
  align-items: center;
  flex-wrap: wrap;
}

.search-group {
  position: relative;
  flex: 1;
  min-width: 200px;
}

.search-icon {
  position: absolute;
  left: 8px;
  top: 50%;
  transform: translateY(-50%);
  color: #666;
  z-index: 1;
}

.dark-theme .search-icon {
  color: #ccc;
}

.search-input {
  width: 100%;
  padding: 6px 12px 6px 32px;
  border: 1px solid #ccc;
  border-radius: 3px;
  font-size: 13px;
  background: white;
}

.search-input:focus {
  outline: none;
  border-color: #0078d4;
  box-shadow: 0 0 0 1px #0078d4;
}

.dark-theme .search-input {
  background: #2d2d2d;
  border-color: #555;
  color: #fff;
}

.dark-theme .search-input:focus {
  border-color: #0078d4;
}

.department-filter {
  padding: 6px 12px;
  border: 1px solid #ccc;
  border-radius: 3px;
  font-size: 13px;
  background: white;
  min-width: 150px;
}

.dark-theme .department-filter {
  background: #2d2d2d;
  border-color: #555;
  color: #fff;
}

.refresh-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #0078d4;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 3px;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.refresh-btn:hover {
  background: #106ebe;
}

/* Main Content */
.main-content {
  flex: 1;
  padding: 16px;
  overflow: auto;
}

.content-container {
  max-width: 1400px;
  margin: 0 auto;
}

.loading-section, .error-section {
  text-align: center;
  padding: 60px 20px;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.dark-theme .loading-section,
.dark-theme .error-section {
  background: #2d2d2d;
  border-color: #555;
  color: #fff;
}

.loading-spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #f3f3f3;
  border-left: 3px solid #0078d4;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.retry-btn {
  background: #d13438;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 3px;
  cursor: pointer;
  margin-top: 16px;
}

.retry-btn:hover {
  background: #a4282c;
}

/* Employee List */
.employee-list {
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  overflow: hidden;
}

.dark-theme .employee-list {
  background: #2d2d2d;
  border-color: #555;
}

.list-header {
  background: #f0f0f0;
  border-bottom: 1px solid #ddd;
  display: grid;
  grid-template-columns: 60px 200px 120px 140px 140px 80px 120px 1fr;
  font-size: 12px;
  font-weight: 600;
  color: #333;
}

.dark-theme .list-header {
  background: #3c3c3c;
  border-bottom-color: #555;
  color: #fff;
}

.header-cell {
  padding: 12px 8px;
  text-align: left;
  border-right: 1px solid #ddd;
}

.dark-theme .header-cell {
  border-right-color: #555;
}

.header-cell:last-child {
  border-right: none;
}

.list-body {
  max-height: 600px;
  overflow-y: auto;
}

.employee-row {
  display: grid;
  grid-template-columns: 60px 200px 120px 140px 140px 80px 120px 1fr;
  border-bottom: 1px solid #eee;
  transition: background-color 0.2s ease;
}

.employee-row:hover {
  background: #f8f8f8;
}

.dark-theme .employee-row {
  border-bottom-color: #444;
}

.dark-theme .employee-row:hover {
  background: #3a3a3a;
}

.employee-row.inactive {
  opacity: 0.6;
}

.cell {
  padding: 8px;
  display: flex;
  align-items: center;
  border-right: 1px solid #eee;
  font-size: 13px;
}

.dark-theme .cell {
  border-right-color: #444;
  color: #fff;
}

.cell:last-child {
  border-right: none;
}

.cell.photo {
  justify-content: center;
}

.employee-photo {
  width: 40px;
  height: 40px;
  border-radius: 20px;
  overflow: hidden;
  background: #f0f0f0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.employee-photo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.photo-placeholder {
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #4FC08D 0%, #44A08D 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: bold;
}

.name-info {
  display: flex;
  flex-direction: column;
}

.full-name {
  font-weight: 500;
  margin-bottom: 2px;
}

.email {
  font-size: 11px;
  color: #666;
}

.dark-theme .email {
  color: #ccc;
}

.status-badge {
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 500;
  text-transform: uppercase;
}

.status-badge.active {
  background: #dff0d8;
  color: #3c763d;
  border: 1px solid #d6e9c6;
}

.status-badge.inactive {
  background: #f2dede;
  color: #a94442;
  border: 1px solid #ebccd1;
}

.dark-theme .status-badge.active {
  background: #2d5016;
  color: #a3d977;
}

.dark-theme .status-badge.inactive {
  background: #5c1e1e;
  color: #f5a9a9;
}

.badge-serial {
  font-family: 'Consolas', 'Courier New', monospace;
  background: #f8f8f8;
  padding: 2px 6px;
  border-radius: 3px;
  border: 1px solid #ddd;
  font-size: 11px;
}

.dark-theme .badge-serial {
  background: #1e1e1e;
  border-color: #555;
}

.access-profiles {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.profile-tag {
  background: linear-gradient(135deg, #4FC08D 0%, #44A08D 100%);
  color: white;
  padding: 2px 6px;
  border-radius: 10px;
  font-size: 10px;
  font-weight: 500;
}

/* Main Layout with Sidebar */
.main-layout {
  display: flex;
  height: calc(100vh - 160px); /* Adjust based on header height */
  gap: 1px;
}

.employee-sidebar {
  width: 320px;
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
}

.dark-theme .employee-sidebar {
  background: #2d2d2d;
  border-color: #555;
}

.sidebar-header {
  padding: 16px;
  border-bottom: 1px solid #e0e0e0;
  background: #f8f9fa;
}

.dark-theme .sidebar-header {
  background: #383838;
  border-color: #555;
}

.sidebar-header h3 {
  margin: 0;
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

.dark-theme .sidebar-header h3 {
  color: #fff;
}

.employee-list-sidebar {
  flex: 1;
  overflow-y: auto;
  padding: 8px;
}

.employee-item {
  padding: 12px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid transparent;
  margin-bottom: 4px;
  position: relative;
}

.employee-item:hover {
  background: #f5f5f5;
  border-color: #e0e0e0;
}

.employee-item.selected {
  background: #e3f2fd;
  border-color: #2196f3;
}

.employee-item.inactive {
  opacity: 0.6;
}

.dark-theme .employee-item:hover {
  background: #383838;
  border-color: #555;
}

.dark-theme .employee-item.selected {
  background: #1e3a5f;
  border-color: #2196f3;
}

.employee-item-content {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.employee-name {
  font-weight: 600;
  font-size: 13px;
  color: #333;
}

.dark-theme .employee-name {
  color: #fff;
}

.employee-details {
  display: flex;
  justify-content: space-between;
  font-size: 11px;
  color: #666;
}

.dark-theme .employee-details {
  color: #ccc;
}

.status-indicator {
  position: absolute;
  top: 8px;
  right: 8px;
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status-indicator.active {
  background: #4caf50;
}

.status-indicator.inactive {
  background: #f44336;
}

/* Employee Details Panel */
.employee-details-panel {
  flex: 1;
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
}

.dark-theme .employee-details-panel {
  background: #2d2d2d;
  border-color: #555;
}

.employee-profile {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.profile-header {
  padding: 24px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  gap: 24px;
  align-items: flex-start;
}

.dark-theme .profile-header {
  border-color: #555;
}

.profile-photo-section {
  flex-shrink: 0;
}

.employee-photo-large {
  width: 120px;
  height: 120px;
  border-radius: 8px;
  overflow: hidden;
  border: 3px solid #e0e0e0;
  background: #f5f5f5;
}

.dark-theme .employee-photo-large {
  border-color: #555;
  background: #383838;
}

.employee-photo-large img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.photo-placeholder-large {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
  font-weight: bold;
  color: #666;
  background: linear-gradient(135deg, #f0f0f0 0%, #e0e0e0 100%);
}

.dark-theme .photo-placeholder-large {
  color: #ccc;
  background: linear-gradient(135deg, #383838 0%, #2d2d2d 100%);
}

.profile-info {
  flex: 1;
}

.employee-full-name {
  margin: 0 0 16px 0;
  font-size: 24px;
  font-weight: 600;
  color: #333;
}

.dark-theme .employee-full-name {
  color: #fff;
}

.employee-meta {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.meta-item {
  font-size: 13px;
  color: #666;
}

.dark-theme .meta-item {
  color: #ccc;
}

/* Tabs */
.profile-tabs {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.tab-headers {
  display: flex;
  border-bottom: 1px solid #e0e0e0;
  background: #f8f9fa;
}

.dark-theme .tab-headers {
  background: #383838;
  border-color: #555;
}

.tab-header {
  background: none;
  border: none;
  padding: 12px 20px;
  cursor: pointer;
  font-size: 13px;
  color: #666;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.2s ease;
  border-bottom: 2px solid transparent;
}

.tab-header:hover {
  background: #e9ecef;
  color: #333;
}

.tab-header.active {
  color: #2196f3;
  border-bottom-color: #2196f3;
  background: #fff;
}

.dark-theme .tab-header {
  color: #ccc;
}

.dark-theme .tab-header:hover {
  background: #2d2d2d;
  color: #fff;
}

.dark-theme .tab-header.active {
  background: #2d2d2d;
  color: #2196f3;
}

.tab-icon {
  font-size: 14px;
}

.tab-content {
  flex: 1;
  overflow-y: auto;
}

.tab-panel {
  padding: 24px;
}

/* Details Grid */
.details-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 32px;
}

.detail-group h4 {
  margin: 0 0 16px 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
  border-bottom: 1px solid #e0e0e0;
  padding-bottom: 8px;
}

.dark-theme .detail-group h4 {
  color: #fff;
  border-color: #555;
}

.detail-item {
  margin-bottom: 12px;
  font-size: 13px;
  color: #666;
}

.dark-theme .detail-item {
  color: #ccc;
}

.access-profiles-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

/* History Lists */

.history-item {
  background: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 6px;
  padding: 16px;
  margin-bottom: 12px;
}

.history-item.denied {
  border-left: 4px solid #f44336;
}

.dark-theme .history-item {
  background: #383838;
  border-color: #555;
}

.history-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.action-type {
  font-weight: 600;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 11px;
  text-transform: uppercase;
}

.action-type.granted {
  background: #dff0d8;
  color: #3c763d;
}

.action-type.denied {
  background: #f2dede;
  color: #a94442;
}

.dark-theme .action-type.granted {
  background: #2d5016;
  color: #a3d977;
}

.dark-theme .action-type.denied {
  background: #5c1e1e;
  color: #f5a9a9;
}

.timestamp {
  font-size: 11px;
  color: #999;
}

.history-details {
  font-size: 12px;
  color: #666;
  line-height: 1.4;
}

.history-details > div {
  margin-bottom: 4px;
}

.dark-theme .history-details {
  color: #ccc;
}

/* No Selection State */
.no-selection {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
}

.no-selection-content {
  text-align: center;
  color: #666;
}

.dark-theme .no-selection-content {
  color: #ccc;
}

.no-selection-icon {
  font-size: 64px;
  margin-bottom: 16px;
  opacity: 0.5;
}

.no-selection-content h3 {
  margin: 0 0 8px 0;
  font-size: 18px;
  font-weight: 600;
}

.no-selection-content p {
  margin: 0;
  font-size: 14px;
  max-width: 300px;
}

/* Responsive Design */
@media (max-width: 1200px) {
  .list-header,
  .employee-row {
    grid-template-columns: 60px 160px 100px 120px 120px 70px 100px 1fr;
  }
}

@media (max-width: 768px) {
  .search-section {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-group,
  .department-filter,
  .refresh-btn {
    width: 100%;
  }
  
  .list-header,
  .employee-row {
    grid-template-columns: 50px 1fr;
  }
  
  .header-cell:not(.photo):not(.name),
  .cell:not(.photo):not(.name) {
    display: none;
  }
  
  .stats-section {
    justify-content: center;
  }
}

/* Scrollbar Styling */
.list-body::-webkit-scrollbar {
  width: 8px;
}

.list-body::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.list-body::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

.list-body::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

.dark-theme .list-body::-webkit-scrollbar-track {
  background: #2d2d2d;
}

.dark-theme .list-body::-webkit-scrollbar-thumb {
  background: #555;
}

.dark-theme .list-body::-webkit-scrollbar-thumb:hover {
  background: #777;
}

/* Card Fields Styles */
.card-fields-content {
  display: flex;
  flex-direction: column;
  gap: 24px;
  padding: 20px;
}

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.summary-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.summary-icon {
  font-size: 24px;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f0f0f0;
  border-radius: 50%;
}

.summary-info {
  flex: 1;
}

.summary-number {
  font-size: 24px;
  font-weight: 600;
  color: #2563eb;
  line-height: 1;
}

.summary-label {
  font-size: 12px;
  color: #666;
  margin-top: 4px;
}

.card-fields-sections {
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.field-section h4 {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 16px;
  color: #333;
  display: flex;
  align-items: center;
  gap: 8px;
}

.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
}

.field-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: all 0.2s ease;
}

.field-card:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transform: translateY(-1px);
}

.field-card.inactive {
  opacity: 0.6;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
  position: relative;
}

.card-icon {
  font-size: 20px;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f0f0f0;
  border-radius: 6px;
}

.card-title {
  font-weight: 600;
  font-size: 16px;
  color: #333;
  flex: 1;
}

.status-indicator {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
}

.status-indicator.active {
  background: #22c55e;
}

.status-indicator.inactive {
  background: #ef4444;
}

.card-details {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-bottom: 12px;
}

.card-details .detail-item {
  font-size: 13px;
  color: #666;
}

.card-details .detail-item strong {
  color: #333;
}

.card-profiles {
  border-top: 1px solid #f0f0f0;
  padding-top: 12px;
}

.profiles-list {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.profile-tag {
  display: inline-block;
  padding: 4px 8px;
  background: #e3f2fd;
  color: #1976d2;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 500;
}

.more-indicator {
  display: inline-block;
  padding: 4px 8px;
  background: #f5f5f5;
  color: #666;
  border-radius: 12px;
  font-size: 11px;
  font-style: italic;
}

.reader-card .card-icon {
  background: #e8f5e8;
  color: #4caf50;
}

.profile-card .card-icon {
  background: #fff3e0;
  color: #ff9800;
}

/* Dark Theme Card Fields */
.dark-theme .summary-card {
  background: #2d2d2d;
  border-color: #444;
  color: #e0e0e0;
}

.dark-theme .summary-icon {
  background: #404040;
}

.dark-theme .summary-number {
  color: #60a5fa;
}

.dark-theme .summary-label {
  color: #aaa;
}

.dark-theme .field-section h4 {
  color: #e0e0e0;
}

.dark-theme .field-card {
  background: #2d2d2d;
  border-color: #444;
  color: #e0e0e0;
}

.dark-theme .card-icon {
  background: #404040;
}

.dark-theme .card-title {
  color: #e0e0e0;
}

.dark-theme .card-details .detail-item {
  color: #aaa;
}

.dark-theme .card-details .detail-item strong {
  color: #e0e0e0;
}

.dark-theme .card-profiles {
  border-color: #444;
}

.dark-theme .profile-tag {
  background: #1e3a8a;
  color: #93c5fd;
}

.dark-theme .more-indicator {
  background: #404040;
  color: #aaa;
}

.dark-theme .reader-card .card-icon {
  background: #1b5e20;
  color: #81c784;
}

.dark-theme .profile-card .card-icon {
  background: #e65100;
  color: #ffb74d;
}

/* New Profile Button */
.new-profile-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.new-profile-btn:hover {
  background: #1d4ed8;
  transform: translateY(-1px);
}

.new-profile-btn:active {
  transform: translateY(0);
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal-content {
  background: white;
  border-radius: 12px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  max-width: 800px;
  width: 90vw;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24px 32px 0 32px;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 16px;
  margin-bottom: 24px;
}

.modal-header h2 {
  font-size: 24px;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  color: #6b7280;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: #f3f4f6;
  color: #374151;
}

.modal-body {
  padding: 0 32px;
  overflow-y: auto;
  flex: 1;
}

.employee-form {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.form-grid {
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.form-section {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 24px;
  background: #f9fafb;
}

.form-section h3 {
  font-size: 18px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 20px 0;
  padding-bottom: 12px;
  border-bottom: 1px solid #e5e7eb;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 20px;
}

.form-row:last-child {
  margin-bottom: 0;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: #374151;
}

.form-input,
.form-select {
  padding: 12px 16px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  transition: all 0.2s ease;
  background: white;
}

.form-input:focus,
.form-select:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.checkbox-group {
  justify-content: center;
  align-items: center;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 14px;
  color: #374151;
}

.form-checkbox {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.checkbox-text {
  font-weight: 500;
}

.modal-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  padding: 24px 32px;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
}

.btn-cancel {
  padding: 10px 20px;
  background: #f3f4f6;
  color: #374151;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-cancel:hover {
  background: #e5e7eb;
}

.btn-save {
  padding: 10px 20px;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  min-width: 120px;
}

.btn-save:hover:not(:disabled) {
  background: #1d4ed8;
}

.btn-save:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

/* Dark Theme Modal */
.dark-theme .modal-content {
  background: #2d2d2d;
  color: #e0e0e0;
}

.dark-theme .modal-header {
  border-color: #444;
}

.dark-theme .modal-header h2 {
  color: #e0e0e0;
}

.dark-theme .modal-close {
  color: #aaa;
}

.dark-theme .modal-close:hover {
  background: #404040;
  color: #e0e0e0;
}

.dark-theme .form-section {
  background: #1e1e1e;
  border-color: #444;
}

.dark-theme .form-section h3 {
  color: #e0e0e0;
  border-color: #444;
}

.dark-theme .form-group label {
  color: #ccc;
}

.dark-theme .form-input,
.dark-theme .form-select {
  background: #404040;
  border-color: #555;
  color: #e0e0e0;
}

.dark-theme .form-input:focus,
.dark-theme .form-select:focus {
  border-color: #60a5fa;
  box-shadow: 0 0 0 3px rgba(96, 165, 250, 0.1);
}

.dark-theme .checkbox-label {
  color: #ccc;
}

.dark-theme .modal-footer {
  background: #1e1e1e;
  border-color: #444;
}

.dark-theme .btn-cancel {
  background: #404040;
  color: #e0e0e0;
  border-color: #555;
}

.dark-theme .btn-cancel:hover {
  background: #555;
}

/* Responsive Design */
@media (max-width: 768px) {
  .modal-content {
    width: 95vw;
    max-height: 95vh;
  }
  
  .modal-header,
  .modal-body,
  .modal-footer {
    padding-left: 20px;
    padding-right: 20px;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .form-section {
    padding: 16px;
  }
}
</style>