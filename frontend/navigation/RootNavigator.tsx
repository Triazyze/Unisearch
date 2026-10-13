import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useEffect } from 'react';
import { ActivityIndicator, StyleSheet, View } from 'react-native';
import AdminMenuScreen from '../screens/Admin/AdminMenuScreen/AdminMenuScreen';
import GridListScreen from '../screens/Admin/GridListScreen/GridListScreen';
import GridDetailScreen from '../screens/Admin/GridDetailScreen/GridDetailScreen';
import GenerateGridScreen from '../screens/Admin/GenerateGridScreen/GenerateGridScreen';
import LoginScreen from '../screens/LoginScreen/LoginScreen';
import MainScreen from '../screens/MainScreen/MainScreen';
import PlayerMenuScreen from '../screens/Player/PlayerMenuScreen/PlayerMenuScreen';
import RegisterScreen from '../screens/RegisterScreen/RegisterScreen';
import { useAuthStore } from '../stores/useAuthStore';
import { theme } from '../components/ui/theme';

export type RootStackParamList = {
  AdminMenu: undefined;
  AdminGridList: undefined;
  AdminGridDetail: { gridId: string };
  AdminGenerateGrid: undefined;
  Login: undefined;
  Main: undefined;
  PlayerMenu: undefined;
  Register: undefined;
};

const Stack = createNativeStackNavigator<RootStackParamList>();

export default function RootNavigator() {
  const isRestoringSession = useAuthStore((state) => state.isRestoringSession);
  const restoreSessionAsync = useAuthStore((state) => state.restoreSessionAsync);
  const user = useAuthStore((state) => state.user);

  useEffect(() => {
    void restoreSessionAsync();
  }, [restoreSessionAsync]);

  if (isRestoringSession) {
    return (
      <View style={styles.loadingScreen}>
        <ActivityIndicator color={theme.colors.primary} size="large" />
      </View>
    );
  }

  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {user === null ? (
          <>
            <Stack.Screen name="Login" component={LoginScreen} />
            <Stack.Screen name="Register" component={RegisterScreen} />
          </>
        ) : (
          <>
            <Stack.Screen name="Main" component={MainScreen} />
            {user.role === 'Player' && (
              <Stack.Screen name="PlayerMenu" component={PlayerMenuScreen} />
            )}
            {user.role === 'Admin' && (
              <>
                <Stack.Screen name="AdminMenu" component={AdminMenuScreen} />
                <Stack.Screen name="AdminGridList" component={GridListScreen} />
                <Stack.Screen name="AdminGridDetail" component={GridDetailScreen} />
                <Stack.Screen name="AdminGenerateGrid" component={GenerateGridScreen} />
              </>
            )}
          </>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}

const styles = StyleSheet.create({
  loadingScreen: {
    alignItems: 'center',
    backgroundColor: theme.colors.white,
    flex: 1,
    justifyContent: 'center',
  },
});
