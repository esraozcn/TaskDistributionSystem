import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { CheckCircle, Clock, AlertCircle, Plus, LogOut, User, Building } from 'lucide-react';
import { clearSession, getUser } from '../services/auth';
import { getWeeklyTasks, generateWeeklyTasks, updateTaskStatus, getTaskStatistics } from '../services/tasks';

export default function Dashboard() {
  const [tasks, setTasks] = useState([]);
  const [statistics, setStatistics] = useState({
    total: 0,
    completed: 0,
    inProgress: 0,
    notStarted: 0
  });
  const [loading, setLoading] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [filterDay, setFilterDay] = useState('all');
  const [filterUser, setFilterUser] = useState('all');
  const [filterTeam, setFilterTeam] = useState('all');
  const navigate = useNavigate();
  const user = getUser();

  useEffect(() => {
    if (user) {
      loadData();
    }
  }, [user?.companyId]); // Sadece companyId değiştiğinde çalışsın

  const loadData = async () => {
    if (!user) return;
    
    setLoading(true);
    try {
      const [tasksData, statsData] = await Promise.all([
        getWeeklyTasks(user.companyId),
        getTaskStatistics(user.companyId)
      ]);
      
      console.log('Tasks Data:', tasksData); // Debug için
      setTasks(tasksData.tasks || []);
      setStatistics(statsData);
    } catch (error) {
      console.error('Veri yüklenirken hata:', error);
      setTasks([]); // Hata durumunda boş array
    } finally {
      setLoading(false);
    }
  };

  const handleGenerateTasks = async () => {
    if (!user) return;
    
    setGenerating(true);
    try {
      await generateWeeklyTasks(user.companyId);
      await loadData();
      alert('Haftalık görevler başarıyla oluşturuldu!');
    } catch (error) {
      alert('Görev oluşturulurken hata: ' + error.message);
    } finally {
      setGenerating(false);
    }
  };

  const handleStatusUpdate = async (taskId, newStatus) => {
    try {
      await updateTaskStatus(taskId, newStatus);
      await loadData();
    } catch (error) {
      alert('Durum güncellenirken hata: ' + error.message);
    }
  };

  const handleLogout = () => {
    clearSession();
    navigate('/login');
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'Completed': return 'bg-green-100 text-green-800';
      case 'InProgress': return 'bg-yellow-100 text-yellow-800';
      case 'NotStarted': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusText = (status) => {
    switch (status) {
      case 'Completed': return 'Tamamlandı';
      case 'InProgress': return 'Devam Ediyor';
      case 'NotStarted': return 'Başlamadı';
      default: return status;
    }
  };

  // Filtrelenmiş görevleri al
  const getFilteredTasks = () => {
    let filtered = tasks;

    // Gün filtresi
    if (filterDay !== 'all') {
      const dayNames = ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
      const dayIndex = dayNames.indexOf(filterDay);
      if (dayIndex !== -1) {
        const targetDate = new Date();
        const monday = new Date(targetDate.setDate(targetDate.getDate() - targetDate.getDay() + 1));
        const filterDate = new Date(monday.getTime() + dayIndex * 24 * 60 * 60 * 1000);
        const dateStr = filterDate.toISOString().split('T')[0];
        
        filtered = filtered.filter(task => task.assignedDate === dateStr);
      }
    }

    // Kullanıcı filtresi
    if (filterUser !== 'all') {
      filtered = filtered.filter(task => task.user.id.toString() === filterUser);
    }

    // Ekip filtresi
    if (filterTeam !== 'all') {
      filtered = filtered.filter(task => task.user.teamName === filterTeam);
    }

    return filtered;
  };

  // Benzersiz kullanıcıları al
  const getUniqueUsers = () => {
    const users = tasks.map(task => task.user);
    return users.filter((user, index, self) => 
      index === self.findIndex(u => u.id === user.id)
    );
  };

  // Benzersiz ekipleri al
  const getUniqueTeams = () => {
    const teams = tasks.map(task => task.user.teamName);
    return [...new Set(teams)];
  };

  if (!user) {
    return <div>Kullanıcı bilgisi bulunamadı</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <div className="flex items-center space-x-4">
              <h1 className="text-2xl font-bold text-gray-900">Görev Dağıtım Sistemi</h1>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2">
                <User className="w-5 h-5 text-gray-500" />
                <span className="text-gray-700">{user.firstName} {user.lastName}</span>
              </div>
              <div className="flex items-center space-x-2">
                <Building className="w-5 h-5 text-gray-500" />
                <span className="text-gray-700">{user.companyName}</span>
              </div>
              <button
                onClick={handleLogout}
                className="flex items-center space-x-2 px-4 py-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <LogOut className="w-4 h-4" />
                <span>Çıkış</span>
              </button>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* İstatistikler */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-lg p-6 shadow-sm border-l-4 border-blue-500">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Toplam Görev</p>
                <p className="text-3xl font-bold text-gray-900">{statistics.total}</p>
              </div>
              <div className="p-3 rounded-full bg-blue-50">
                <AlertCircle className="h-6 w-6 text-blue-600" />
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg p-6 shadow-sm border-l-4 border-green-500">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Tamamlanan</p>
                <p className="text-3xl font-bold text-gray-900">{statistics.completed}</p>
              </div>
              <div className="p-3 rounded-full bg-green-50">
                <CheckCircle className="h-6 w-6 text-green-600" />
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg p-6 shadow-sm border-l-4 border-yellow-500">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Devam Eden</p>
                <p className="text-3xl font-bold text-gray-900">{statistics.inProgress}</p>
              </div>
              <div className="p-3 rounded-full bg-yellow-50">
                <Clock className="h-6 w-6 text-yellow-600" />
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg p-6 shadow-sm border-l-4 border-red-500">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600">Başlamamış</p>
                <p className="text-3xl font-bold text-gray-900">{statistics.notStarted}</p>
              </div>
              <div className="p-3 rounded-full bg-red-50">
                <AlertCircle className="h-6 w-6 text-red-600" />
              </div>
            </div>
          </div>
        </div>

        {/* Görevler */}
        <div className="bg-white rounded-lg shadow-sm">
          <div className="px-6 py-4 border-b border-gray-200">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-semibold text-gray-900">Haftalık Görevler</h2>
              <button
                onClick={handleGenerateTasks}
                disabled={generating}
                className="flex items-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                <Plus className="w-4 h-4" />
                <span>{generating ? 'Oluşturuluyor...' : 'Haftalık Oluştur'}</span>
              </button>
            </div>

            {/* Filtreler */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Gün Filtresi */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Gün</label>
                <select
                  value={filterDay}
                  onChange={(e) => setFilterDay(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="all">Tüm Günler</option>
                  <option value="Pazartesi">Pazartesi</option>
                  <option value="Salı">Salı</option>
                  <option value="Çarşamba">Çarşamba</option>
                  <option value="Perşembe">Perşembe</option>
                  <option value="Cuma">Cuma</option>
                  <option value="Cumartesi">Cumartesi</option>
                </select>
              </div>

              {/* Kullanıcı Filtresi */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Kullanıcı</label>
                <select
                  value={filterUser}
                  onChange={(e) => setFilterUser(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="all">Tüm Kullanıcılar</option>
                  {getUniqueUsers().map((user) => (
                    <option key={user.id} value={user.id}>
                      {user.name}
                    </option>
                  ))}
                </select>
              </div>

              {/* Ekip Filtresi */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Ekip</label>
                <select
                  value={filterTeam}
                  onChange={(e) => setFilterTeam(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="all">Tüm Ekipler</option>
                  {getUniqueTeams().map((team) => (
                    <option key={team} value={team}>
                      {team}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>

          <div className="p-6">
            {loading ? (
              <div className="text-center py-8">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
                <p className="mt-2 text-gray-500">Yükleniyor...</p>
              </div>
            ) : tasks.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-gray-500">Henüz görev bulunmuyor. Haftalık görev oluşturun.</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {getFilteredTasks().map((task) => (
                  <div key={task.id} className="bg-gray-50 rounded-lg p-4 border">
                    <div className="flex justify-between items-start mb-3">
                      <div>
                        <h3 className="font-semibold text-gray-900">{task.title}</h3>
                        <p className="text-sm text-gray-600">{task.user.name}</p>
                        <p className="text-xs text-blue-600 font-medium">{task.user.teamName}</p>
                      </div>
                      <span className={`px-2 py-1 text-xs rounded-full ${getStatusColor(task.status)}`}>
                        {getStatusText(task.status)}
                      </span>
                    </div>
                    
                    <p className="text-sm text-gray-600 mb-3">{task.description}</p>
                    
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-xs text-gray-500">
                        {new Date(task.assignedDate).toLocaleDateString('tr-TR')}
                      </span>
                      <span className="text-xs bg-purple-100 text-purple-800 px-2 py-1 rounded-full">
                        Zorluk: {task.difficultyLevel}
                      </span>
                    </div>
                    
                    <div className="flex justify-between items-center">
                      
                      {task.user.id === user.id && (
                        <select
                          value={task.status}
                          onChange={(e) => handleStatusUpdate(task.id, e.target.value)}
                          className="text-xs px-2 py-1 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                        >
                          <option value="NotStarted">Başlamadı</option>
                          <option value="InProgress">Devam Ediyor</option>
                          <option value="Completed">Tamamlandı</option>
                        </select>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
