import './get-animation.css'
import Lottie from 'react-lottie';
import animationData from './lotties/loading.json';

export function Loading() {
    const defaultOptions = {
        loop: true,
        autoplay: true,
        animationData: animationData,
        rendererSettings: {
          preserveAspectRatio: "xMidYMid slice"
        }
      };
    
    return (
      <div>
        <Lottie 
          options={defaultOptions}
          height={300}
          width={300}
        />
      </div>
    );
}