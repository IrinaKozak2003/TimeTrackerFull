import React, { useContext, useEffect, useState } from "react";
import { Container } from "react-bootstrap";
import { fetchPackages } from "../http/packageApi";
import { Context } from "../index";
import { observer } from "mobx-react-lite";
import PackageList from "../components/PackageList";
import { getAllPackages } from "../http/packageApi";

const Main = observer(() => {
  const { user, user_package } = useContext(Context);
  const [isLoading, setIsLoading] = useState(true); // Добавляем состояние для отслеживания загрузки данных

  useEffect(() => {
    const fetchData = async () => {
      try {
      
        if (user.isAdmin) {
          // Проверяем, что user и user.IsAdmin существуют
          await getAllPackages().then((data) => {
         
            user_package.setPackages(data);
          });
        } else  {
          await fetchPackages(user.user.id).then((data) => {
            console.log(data);
            user_package.setPackages(data);
          });
        }
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false); // Завершаем загрузку данных
      }
    };

    fetchData();
  }, [user, user_package]);

  if (isLoading) {
    // Пока данные загружаются, отображаем индикатор загрузки или другое сообщение
    return <div>Loading...</div>;
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', margin: '10px', fontSize: '1.2rem' }}>
        My packages
      </div>
      <Container>
        <PackageList packages={user_package.packages} />
      </Container>
    </div>
  );
});

export default Main;


